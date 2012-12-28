To get MonoNaCl you must add this source to the NaCl sdk:
./naclsdk sources --add https://commondatastorage.googleapis.com/nativeclient-mirror/nacl/nacl_sdk/naclmono_manifest.json

To list current SDK versions:
./naclsdk list

To install an SDK version (Replace '_20' with whatever version needed):
./naclsdk install pepper_20 naclmono_20 naclmono_samples


-----------------------------------
< SharpZipLib >
To load '.bmpc' textures in NaCl you must reference "ICSharpCode.SharpZipLib.dll"