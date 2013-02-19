import os
import time

import bpy
import mathutils
import bpy_extras.io_utils


def save(operator, context, filepath="", path_mode='AUTO'):
    
    # open file
    file = open(filepath, "w", encoding="utf8", newline="\n")
    
    # define globals
    scene = context.scene
    world = scene.world
    objects = scene.objects # context.selected_objects
    
    # write XML header
    file.write("<?xml version=\"1.0\" encoding=\"utf-8\"?>\n")
    
    # begin writing scene
    file.write("<!-- Reign Model-XML Format -->\n")
    file.write("<!-- www.reign-studios.com -->\n")
    file.write("<Scene Version=\"1.0\">\n")

    # write materials
    file.write("\t<Materials>\n")

    file.write("\t\t<Material Name=\"TestMat\">\n")
    file.write("\t\t\t<Input ID=\"Diffuse\" Type=\"Value\">")
    file.write("1 0 0 1")
    file.write("</Input>\n")
    file.write("\t\t</Material>\n")

    file.write("\t</Materials>\n")
    
    # for each object in scene
    for obj in objects:
		
		# begin object
        file.write("\t<Object Name=\"%s\">\n" % obj.name)

        # write transform
        file.write("\t\t<Transform>\n")
        file.write("\t\t\t<Input Type=\"EulerRotation\">")
        file.write("0 0 0")
        file.write("</Input>\n")

        file.write("\t\t\t<Input Type=\"Scale\">")
        file.write("1 1 1")
        file.write("</Input>\n")

        file.write("\t\t\t<Input Type=\"Position\">")
        file.write("0 0 0")
        file.write("</Input>\n")
        file.write("\t\t</Transform>\n")
        
        try:
            mesh = obj.to_mesh(scene, True, 'PREVIEW')
        except RuntimeError:
            mesh = None
        
        if mesh is not None and (len(mesh.vertices) + len(mesh.tessfaces)) > 0:
		    # begin mesh
            file.write("\t\t<Mesh Material=\"TestMat\">\n")
                
            # write vertices
            file.write("\t\t\t<Vertices>\n")

            # --positions
            file.write("\t\t\t\t<Channel ID=\"Position\" Step=\"3\">")
            for vert in mesh.vertices:
                file.write("%.6f %.6f %.6f " % vert.co[:])
            file.write("</Channel>\n")

            # --normals
            file.write("\t\t\t\t<Channel ID=\"Normal\" Step=\"3\">")
            for face in mesh.tessfaces:
                if face.use_smooth:
                    for i in face.vertices:
                        file.write("%.6f %.6f %.6f " % mesh.vertices[i].normal[:])
                else:
                    file.write("%.6f %.6f %.6f " % face.normal[:])
            file.write("</Channel>\n")

            file.write("\t\t\t</Vertices>\n")
            
            # write faces
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
                
            file.write("\t\t\t</Faces>\n")
            
            # end mesh
            file.write("\t\t</Mesh>\n")
        
        # end object
        file.write("\t</Object>\n")
    
    # end writing scene and file
    file.write("</Scene>\n")
    file.close()
    
    return {'FINISHED'}
