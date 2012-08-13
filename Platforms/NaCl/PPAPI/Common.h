#pragma once

#include <stdbool.h>
#include <string.h>
#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>

#include <mono/jit/jit.h>
#include <mono/metadata/assembly.h>
#include <mono/metadata/debug-helpers.h>

#include "ppapi/c/pp_var.h"
#include "ppapi/c/pp_module.h"
#include "ppapi/c/pp_completion_callback.h"
#include "ppapi/c/pp_errors.h"
#include "ppapi/c/pp_instance.h"

#include "ppapi/c/ppb.h"
#include "ppapi/c/ppb_var.h"
#include "ppapi/c/ppb_core.h"
#include "ppapi/c/ppb_url_loader.h"
#include "ppapi/c/ppb_url_request_info.h"
#include "ppapi/c/ppb_instance.h"
#include "ppapi/c/ppb_messaging.h"

#include "ppapi/c/ppp.h"
#include "ppapi/c/ppp_instance.h"
#include "ppapi/c/ppp_messaging.h"

#define assert(__cond) \
  do { \
    if (!(__cond)) { \
      fprintf(stderr, \
              "condition failed! " #__cond " %s:%d\n", __FILE__, __LINE__); \
    } \
  } while (0)

