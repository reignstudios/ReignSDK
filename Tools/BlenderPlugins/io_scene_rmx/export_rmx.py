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

        if mesh.id_data.users < 1 or (len(mesh.vertices) + len(mesh.polygons)) <= 0:
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
        for poly in mesh.polygons:
            if poly.use_smooth:
                for i in poly.vertices:
                    file.write("%.6f %.6f %.6f " % mesh.vertices[i].normal[:])
            else:
                file.write("%.6f %.6f %.6f " % poly.normal[:])
        file.write("</Channel>\n")

        # --uv
        hasUVs = len(mesh.uv_layers) > 0
        if hasUVs:
            index = 0
            for poly in mesh.uv_layers:
                file.write("\t\t\t\t<Channel ID=\"UV\" Index=\"%d\" Step=\"2\">" % index)
                for uvData in poly.data:
                    file.write("%.6f %.6f " % uvData.uv[:])

                index += 1
                file.write("</Channel>\n")

        # --bone groups/weights
        file.write("\t\t\t\t<BoneGroups>\n")
        file.write("\t\t\t\t\t<Counts>")
        for vert in mesh.vertices:
            file.write("%d " % len(vert.groups))
        file.write("</Counts>\n")
        file.write("\t\t\t\t\t<Indices>")
        for vert in mesh.vertices:
            for group in vert.groups:
                file.write("%d " % group.group)
        file.write("</Indices>\n")
        file.write("\t\t\t\t\t<Weights>")
        for vert in mesh.vertices:
            for group in vert.groups:
                file.write("%.6f " % group.weight)
        file.write("</Weights>\n")
        file.write("\t\t\t\t</BoneGroups>\n")

        file.write("\t\t\t</Vertices>\n")
        
        # write faces -------------------------
        file.write("\t\t\t<Faces>\n")
        
        # write face steps
        file.write("\t\t\t\t<Steps>")
        for poly in mesh.polygons:
            file.write("%d " % len(poly.vertices))
        file.write("</Steps>\n")
        
        # write face position indices
        file.write("\t\t\t\t<Indices ID=\"Position\">")
        for poly in mesh.polygons:
            for i in range(len(poly.vertices)):
                file.write("%d " % poly.vertices[i])
        file.write("</Indices>\n")

        # write face normal indices
        file.write("\t\t\t\t<Indices ID=\"Normal\">")
        i2 = 0
        for poly in mesh.polygons:
            if poly.use_smooth:
                vertLength = len(poly.vertices)
                for i in range(vertLength):
                    file.write("%d " % (i2 + i))

                i2 = i2 + vertLength
            else:
                vertLength = len(poly.vertices)
                for i in range(vertLength):
                    file.write("%d " % i2)

                i2 = i2 + 1
        file.write("</Indices>\n")

        # write face uv indices
        if hasUVs:
            file.write("\t\t\t\t<Indices ID=\"UV\">")
            i2 = 0
            for poly in mesh.polygons:
                vertLength = len(poly.vertices)
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
            aType = "OBJECT"
            if m is not None:
                dataPath = m.group(1)
                aType = "BONE"
            file.write("\t\t\t<FCurve Type=\"%s\" DataPath=\"%s\" Index=\"%d\">\n" % (aType, dataPath, f.array_index))

            file.write("\t\t\t\t<Coordinates>")
            for k in f.keyframe_points:
                file.write("%.6f %.6f " % k.co[:])
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

            file.write("\t\t\t</FCurve>\n")

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

        file.write("\t\t\t<Bones>\n")
        for bone in arm.bones:
            if bone.use_deform == False:
                continue

            parentName = str()
            if bone.parent != None:
                parentName = bone.parent.name
            file.write("\t\t\t\t<Bone Name=\"%s\" Parent=\"%s\">\n" % (bone.name, parentName))

            file.write("\t\t\t\t\t<InheritScale>%s</InheritScale>\n" % bone.use_inherit_scale)
            file.write("\t\t\t\t\t<InheritRotation>%s</InheritRotation>\n" % bone.use_inherit_rotation)

            boneX = bone.tail[0] + bone.head[0]
            boneY = bone.tail[1] + bone.head[1]
            boneZ = bone.tail[2] + bone.head[2]
            file.write("\t\t\t\t\t<Position>%.6f %.6f %.6f</Position>\n" % (boneX, boneY, boneZ))

            file.write("\t\t\t\t\t<Orientation>%.6f %.6f %.6f " % bone.matrix[0][:])
            file.write("%.6f %.6f %.6f " % bone.matrix[1][:])
            file.write("%.6f %.6f %.6f</Orientation>\n" % bone.matrix[2][:])

            file.write("\t\t\t\t</Bone>\n")

        file.write("\t\t\t</Bones>\n")

        file.write("\t\t</Armature>\n")
    file.write("\t</Armatures>\n")
    
    # -------------------------------------
    # write objects -----------------------
    # -------------------------------------
    file.write("\t<Objects>\n")
    for obj in objects:
		
		# begin object
        parentName = str()
        if obj.parent != None:
            parentName = obj.parent.name
        file.write("\t\t<Object Name=\"%s\" Type=\"%s\" Parent=\"%s\">\n" % (obj.name, obj.type, parentName))

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
        
        # write mesh link
        if obj.type == "MESH":
            mesh = obj.data
            if (len(mesh.vertices) + len(mesh.tessfaces)) > 0:
                file.write("\t\t\t<Mesh Name=\"%s\"/>\n" % mesh.name)
            arm = obj.find_armature()
            if arm is not None:
                file.write("\t\t\t<ArmatureObject Name=\"%s\"/>\n" % arm.name)

        # write armature link
        if obj.type == "ARMATURE":
            armature = obj.data
            file.write("\t\t\t<Armature Name=\"%s\"/>\n" % armature.name)

        # write vertex groups
        if len(obj.vertex_groups) > 0:
            file.write("\t\t\t<BoneGroups>\n")
            for group in obj.vertex_groups:
                file.write("\t\t\t\t<BoneGroup Name=\"%s\" Index=\"%d\"/>\n" % (group.name, group.index))
            file.write("\t\t\t</BoneGroups>\n")

        # write armature link
        if obj.animation_data is not None and obj.animation_data.action is not None:
            file.write("\t\t\t<DefaultAction Name=\"%s\"/>\n" % obj.animation_data.action.name)

        # end object
        file.write("\t\t</Object>\n")

    file.write("\t</Objects>\n")
    
    # end writing scene and file
    file.write("</Scene>\n")
    file.close()
    
    return {'FINISHED'}
