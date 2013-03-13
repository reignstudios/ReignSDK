import os
import time
import re
import mathutils

import bpy
import bpy_extras.io_utils


def save(operator, context, filepath="", path_mode='AUTO'):
    
    # open file
    file = open(filepath, "w", encoding="utf8", newline="\n")
    
    # define globals
    scene = context.scene
    objects = scene.objects
    
    # write XML header
    file.write("<?xml version=\"1.0\" encoding=\"utf-8\"?>\n")
    
    # begin writing scene
    file.write("<!-- Reign Model-XML Format -->\n")
    file.write("<!-- www.reign-studios.com -->\n")
    file.write("<Scene Version=\"1.0\">\n")

    # -------------------------------------
    # write materials ---------------------
    # -------------------------------------
    file.write("\t<Materials>\n")

    # Reference http://www.blender.org/documentation/blender_python_api_2_66_release/bpy.types.Material.html#bpy.types.Material
    for mtl in bpy.data.materials:
        if mtl.id_data.users < 1:
            continue

        file.write("\t\t<Material Name=\"%s\">\n" % mtl.name)

        file.write("\t\t\t<Input ID=\"Alpha\" Type=\"Value\">")
        file.write("%.6f" % mtl.alpha)
        file.write("</Input>\n")

        file.write("\t\t\t<Input ID=\"Diffuse\" Type=\"Value\">")
        file.write("%.6f %.6f %.6f" % mtl.diffuse_color[:])
        file.write("</Input>\n")

        file.write("\t\t\t<Input ID=\"Specular\" Type=\"Value\">")
        file.write("%.6f %.6f %.6f" % mtl.specular_color[:])
        file.write("</Input>\n")

        file.write("\t\t\t<Input ID=\"SpecularSharpness\" Type=\"Value\">")
        file.write("%.6f" % mtl.specular_hardness)
        file.write("</Input>\n")

        file.write("\t\t\t<Input ID=\"SpecularIntensity\" Type=\"Value\">")
        file.write("%.6f" % mtl.specular_intensity)
        file.write("</Input>\n")

        for materialTextureSlot in mtl.texture_slots:
            if materialTextureSlot is not None and issubclass(type(materialTextureSlot.texture), bpy.types.ImageTexture):
                file.write("\t\t\t<Input ID=\"Diffuse\" Type=\"Texture\">")
                texture = bpy.types.ImageTexture(materialTextureSlot.texture)
                file.write(texture.image.filepath)
                file.write("</Input>\n")

        file.write("\t\t</Material>\n")

    file.write("\t</Materials>\n")

    # -------------------------------------
    # write meshes ------------------------
    # -------------------------------------
    file.write("\t<Meshes>\n")
    for mesh in bpy.data.meshes:

        if mesh.id_data.users < 1 or (len(mesh.vertices) + len(mesh.tessfaces)) <= 0:
            continue

        # add material to list if needed
        materialName = str()
        if len(mesh.materials) > 0:
            materialName = mesh.materials[0].name

        # begin mesh
        file.write("\t\t<Mesh Name=\"%s\" Material=\"%s\">\n" % (mesh.name, materialName))
            
        # write vertices ----------------------
        file.write("\t\t\t<Vertices>\n")

        # --positions
        file.write("\t\t\t\t<Channel ID=\"Position\" Index=\"0\" Step=\"3\">")
        for vert in mesh.vertices:
            file.write("%.6f %.6f %.6f " % vert.co[:])
        file.write("</Channel>\n")

        # --normals
        file.write("\t\t\t\t<Channel ID=\"Normal\" Index=\"0\" Step=\"3\">")
        for face in mesh.tessfaces:
            if face.use_smooth:
                for i in face.vertices:
                    file.write("%.6f %.6f %.6f " % mesh.vertices[i].normal[:])
            else:
                file.write("%.6f %.6f %.6f " % face.normal[:])
        file.write("</Channel>\n")

        # --uv
        hasUVs = len(mesh.uv_textures) > 0
        if hasUVs:
            index = 0
            for face in mesh.tessface_uv_textures:
                file.write("\t\t\t\t<Channel ID=\"UV\" Index=\"%d\" Step=\"2\">" % index)
                #for uvData in mesh.tessface_uv_textures.active.data: # Use if you only want to use one UV set
                for uvData in face.data:
                    for uv in uvData.uv:
                        file.write("%.6f %.6f " % uv[:])

                index += 1
                file.write("</Channel>\n")

        file.write("\t\t\t</Vertices>\n")
        
        # write faces -------------------------
        file.write("\t\t\t<Faces>\n")
        
        # write face steps
        file.write("\t\t\t\t<Steps>")
        for face in mesh.tessfaces:
            file.write("%d " % len(face.vertices))
        file.write("</Steps>\n")
        
        # write face position indices
        file.write("\t\t\t\t<Indices ID=\"Position\">")
        for face in mesh.tessfaces:
            for i in range(len(face.vertices)):
                file.write("%d " % face.vertices[i])
        file.write("</Indices>\n")

        # write face normal indices
        file.write("\t\t\t\t<Indices ID=\"Normal\">")
        i2 = 0
        for face in mesh.tessfaces:
            if face.use_smooth:
                vertLength = len(face.vertices)
                for i in range(vertLength):
                    file.write("%d " % (i2 + i))

                i2 = i2 + vertLength
            else:
                vertLength = len(face.vertices)
                for i in range(vertLength):
                    file.write("%d " % i2)

                i2 = i2 + 1
        file.write("</Indices>\n")

        # write face uv indices
        if hasUVs:
            file.write("\t\t\t\t<Indices ID=\"UV\">")
            i2 = 0
            for face in mesh.tessfaces:
                vertLength = len(face.vertices)
                for i in range(vertLength):
                    file.write("%d " % (i2 + i))

                i2 = i2 + vertLength
            file.write("</Indices>\n")
            
        file.write("\t\t\t</Faces>\n")
        
        # end mesh
        file.write("\t\t</Mesh>\n")

    file.write("\t</Meshes>\n")

    # -------------------------------------
    # write actions -----------------------
    # -------------------------------------
    file.write("\t<Actions>\n")
    for a in bpy.data.actions:
        if a.id_data.users < 1:
            continue

        file.write("\t\t<Action Name=\"%s\">\n" % a.name)

        # write fcurves-----------------
        for f in a.fcurves:
            dataPath = f.data_path
            m = re.search('pose.bones\["([\w|\s|\.]*)"\]', dataPath)
            if m is not None:
                dataPath = m.group(1)
            file.write("\t\t\t<FCurves DataPath=\"%s\" Index=\"%d\">\n" % (dataPath, f.array_index))

            file.write("\t\t\t\t<Coordinates>")
            for k in f.keyframe_points:
                file.write("%.6f %.6f " % (k.co[0], k.co[1]))
            file.write("</Coordinates>\n")

            file.write("\t\t\t\t<InterpolationType>")
            for k in f.keyframe_points:
                if k.interpolation == "BEZIER":
                    file.write("B")
                elif k.interpolation == "LINEAR":
                    file.write("L")
                elif k.interpolation == "CONSTANT":
                    file.write("C")
            file.write("</InterpolationType>\n")

            file.write("\t\t\t</FCurves>\n")

        file.write("\t\t</Action>\n")
    file.write("\t</Actions>\n")

    # -------------------------------------
    # write armatures ---------------------
    # -------------------------------------
    file.write("\t<Armatures>\n")
    for arm in bpy.data.armatures:
        if arm.id_data.users < 1:
            continue

        file.write("\t\t<Armature Name=\"%s\">\n" % arm.name)



        file.write("\t\t</Armature>\n")
    file.write("\t</Armatures>\n")
    
    # -------------------------------------
    # write objects -----------------------
    # -------------------------------------
    meshList = list()
    armatureList = list()
    file.write("\t<Objects>\n")
    for obj in objects:
		
		# begin object
        file.write("\t\t<Object Name=\"%s\" Type=\"%s\">\n" % (obj.name, obj.type))

        # write transform
        file.write("\t\t\t<Transform>\n")
        file.write("\t\t\t\t<Input Type=\"EulerRotation\">")
        file.write("%.6f " % obj.rotation_euler.x)
        file.write("%.6f " % obj.rotation_euler.y)
        file.write("%.6f" % obj.rotation_euler.z)
        file.write("</Input>\n")

        file.write("\t\t\t\t<Input Type=\"Scale\">")
        file.write("%.6f " % obj.scale.x)
        file.write("%.6f " % obj.scale.y)
        file.write("%.6f" % obj.scale.z)
        file.write("</Input>\n")

        file.write("\t\t\t<Input Type=\"Position\">")
        file.write("%.6f " % obj.location.x)
        file.write("%.6f " % obj.location.y)
        file.write("%.6f" % obj.location.z)
        file.write("</Input>\n")
        file.write("\t\t\t</Transform>\n")
        
        # write link mesh
        if obj.type == "MESH":
            mesh = obj.data
            if (len(mesh.vertices) + len(mesh.tessfaces)) > 0:
                file.write("\t\t\t<Mesh Name=\"%s\"/>\n" % mesh.name)
                if mesh not in meshList:
                    meshList.append(mesh)

        # write link armature
        if obj.type == "ARMATURE":
            armature = obj.data
            file.write("\t\t\t<Armature Name=\"%s\"/>\n" % armature.name)

        # end object
        file.write("\t\t</Object>\n")

    file.write("\t</Objects>\n")
    
    # end writing scene and file
    file.write("</Scene>\n")
    file.close()
    
    return {'FINISHED'}
