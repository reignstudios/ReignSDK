bl_info = {
    "name": "Export: Reign Model-XML Format",
    "author": "Andrew and Philip Witte <zezba9000@gmail.com>",
    "blender": (2, 5, 8),
    "location": "File > Import-Export",
    "description": "Export RMX: Mesh, Material, Textures, & Animation.",
    "warning": "",
    "wiki_url": "http://www.reign-studios.com/",
    "tracker_url": "",
    "support": 'OFFICIAL',
    "category": "Import-Export"}

if "bpy" in locals():
    import imp
    
    if "export_rmx" in locals():
        imp.reload(export_rmx)


import bpy

from bpy.props import (BoolProperty,
                       FloatProperty,
                       StringProperty,
                       EnumProperty,
                       )
from bpy_extras.io_utils import (ExportHelper,
                                 path_reference_mode,
                                 axis_conversion,
                                 )


class ExportRMX(bpy.types.Operator, ExportHelper):
    """Save a Reign Model-XML File"""
    
    bl_idname = "export_scene.rmx"
    bl_label = 'Export RMX'
    bl_options = {'PRESET'}
    
    filename_ext = ".rmx"
    filter_glob = StringProperty(
            default="*.rmx",
            options={'HIDDEN'},
            )
    
    path_mode = path_reference_mode
    check_extension = True
    
    def execute(self, context):
        from . import export_rmx
        
        keywords = self.as_keywords(ignore=("check_existing", "filter_glob"))
        
        return export_rmx.save(self, context, **keywords)


def menu_func_export(self, context):
    self.layout.operator(ExportRMX.bl_idname, text="Reign (.rmx)")


def register():
    bpy.utils.register_module(__name__)
    bpy.types.INFO_MT_file_export.append(menu_func_export)


def unregister():
    bpy.utils.unregister_module(__name__)
    bpy.types.INFO_MT_file_export.remove(menu_func_export)


if __name__ == "__main__":
    register()
