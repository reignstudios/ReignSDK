#include <ppapi/c/ppb_graphics_3d.h>
#include <ppapi/c/ppb_instance.h>
#include <ppapi/c/ppb_opengles2.h>
#include <stdbool.h>
#include "MonoInitialization.h"

PPB_Graphics3D* g_graphics;
PP_Resource g_context;
struct PPB_OpenGLES2* g_gles;
bool isRendering = false;

#pragma region ppapi
PP_Resource InitOpenGL(PP_Instance instance, PPB_Graphics3D* graphics, PPB_Instance* pbbInstance, const int32_t* attribs, struct PPB_OpenGLES2* gles)
{
	PP_Resource context = graphics->Create(instance, 0, attribs);
	pbbInstance->BindGraphics(instance, context);
	
	g_gles = gles;
	g_graphics = graphics;
	g_context = context;
	return context;
}

void swapBufferCallback(void* data, int32_t result)
{
	if (!isRendering) return;
	Mono_InvokeMethodOnMainThread("Reign.Core", "Reign.Core.OS:updateAndRender");
	if (!isRendering) return;
	struct PP_CompletionCallback callBack =
	{
		&swapBufferCallback,
		0
	};
	g_gles->Finish(g_context);
	g_gles->Flush(g_context);
	g_graphics->SwapBuffers(g_context, callBack);
}

void StartSwapBufferLoop(PPB_Graphics3D* graphics, PP_Resource context)
{
	isRendering = true;
	Mono_InvokeMethodOnMainThread("Reign.Core", "Reign.Core.OS:updateAndRender");
	struct PP_CompletionCallback callBack =
	{
		&swapBufferCallback,
		0
	};
	g_gles->Finish(context);
	g_gles->Flush(context);
	graphics->SwapBuffers(context, callBack);
}

void StopSwapBufferLoop()
{
	isRendering = false;
}

void SetCurrentContextPPAPI(PP_Resource context)
{
	glSetCurrentContextPPAPI(context);
}
#pragma endregion

#pragma region GL
typedef char GLchar;

// ---------------------------------------------
// Shaders
// ---------------------------------------------
void glTexParameteri(GLenum target, GLenum pname, GLint param) {g_gles->TexParameteri(g_context, target, pname, param);}
void glTexParameteriv(GLenum target, GLenum pname, const GLint *params) {g_gles->TexParameteriv(g_context, target, pname, params);}
void glGenerateMipmap(GLenum target) {g_gles->GenerateMipmap(g_context, target);}
GLuint glCreateShader(GLenum type) {return g_gles->CreateShader(g_context, type);}
void glShaderSource(GLuint shader, GLsizei count, const GLchar** string, const GLint* length) {g_gles->ShaderSource(g_context, shader, count, string, length);}
void glGetShaderiv(GLuint shader, GLenum pname, GLint* pParams) {g_gles->GetShaderiv(g_context, shader, pname, pParams);}
void glCompileShader(GLuint shader) {g_gles->CompileShader(g_context, shader);}
void glDeleteShader(GLuint shader) {g_gles->DeleteShader(g_context, shader);}
void glGetShaderInfoLog(GLuint shader, GLsizei bufSize, GLsizei* length, GLchar* infoLog) {g_gles->GetShaderInfoLog(g_context, shader, bufSize, length, infoLog);}
GLuint glCreateProgram() {return g_gles->CreateProgram(g_context);}
void glDeleteProgram(GLuint program) {g_gles->DeleteProgram(g_context, program);}
void glAttachShader(GLuint program, GLuint shader) {g_gles->AttachShader(g_context, program, shader);}
void glLinkProgram(GLuint program) {g_gles->LinkProgram(g_context, program);}
void glUseProgram(GLuint program) {g_gles->UseProgram(g_context, program);}
GLint glGetUniformLocation(GLuint program, const GLchar* name) {return g_gles->GetUniformLocation(g_context, program, name);}
void glUniform1f(GLint location, GLfloat x) {g_gles->Uniform1f(g_context, location, x);}
void glUniform2f(GLint location, GLfloat x, GLfloat y) {g_gles->Uniform2f(g_context, location, x, y);}
void glUniform3f(GLint location, GLfloat x, GLfloat y, GLfloat z) {g_gles->Uniform3f(g_context, location, x, y, z);}
void glUniform4f(GLint location, GLfloat x, GLfloat y, GLfloat z, GLfloat w) {g_gles->Uniform4f(g_context, location, x, y, z, w);}
void glUniformMatrix2fv(GLint location, GLsizei count, GLboolean transpose, GLfloat* value) {g_gles->UniformMatrix2fv(g_context, location, count, transpose, value);}
void glUniformMatrix3fv(GLint location, GLsizei count, GLboolean transpose, GLfloat* value) {g_gles->UniformMatrix3fv(g_context, location, count, transpose, value);}
void glUniformMatrix4fv(GLint location, GLsizei count, GLboolean transpose, GLfloat* value) {g_gles->UniformMatrix4fv(g_context, location, count, transpose, value);}
void glUniform1i(GLint location, GLint x) {g_gles->Uniform1i(g_context, location, x);}
void glUniform1fv(GLint location, GLsizei count, GLfloat* v) {g_gles->Uniform1fv(g_context, location, count, v);}
void glUniform2fv(GLint location, GLsizei count, GLfloat* v) {g_gles->Uniform2fv(g_context, location, count, v);}
void glUniform3fv(GLint location, GLsizei count, GLfloat* v) {g_gles->Uniform3fv(g_context, location, count, v);}
void glUniform4fv(GLint location, GLsizei count, GLfloat* v) {g_gles->Uniform4fv(g_context, location, count, v);}

// ---------------------------------------------
// Buffers
// ---------------------------------------------
void glDrawArrays(GLenum mode, GLint first, GLsizei count) {g_gles->DrawArrays(g_context, mode, first, count);}
void glDrawElements(GLenum mode, GLsizei count, GLenum type, const GLvoid* indices) {g_gles->DrawElements(g_context, mode, count, type, indices);}
void glCullFace(GLenum mode) {g_gles->CullFace(g_context, mode);}
// Not in GLES2 - void glPolygonMode(GLenum face, GLenum mode) {g_gles->PolygonMode(g_context, face, mode);}
void glFrontFace(GLenum mode) {g_gles->FrontFace(g_context, mode);}
// Not in GLES2 - void glDrawArraysInstanced(GLenum mode, GLint first, GLsizei count, GLsizei primcount) {g_gles->DrawArraysInstanced(g_context, mode, first, count, primcount);}
// Not in GLES2 - void glDrawElementsInstanced(GLenum mode, GLsizei count, GLenum type, const void * indices, GLsizei primcount) {g_gles->DrawElementsInstanced(g_context, mode, count, type, indices, primcount);}
// Not in GLES2 - void glVertexAttribDivisor(GLuint index, GLuint divisor) {g_gles->VertexAttribDivisor(g_context, index, divisor);}
void glGenBuffers(GLsizei n, GLuint* buffers) {g_gles->GenBuffers(g_context, n, buffers);}
void glBindBuffer(GLenum target, GLuint buffer) {g_gles->BindBuffer(g_context, target, buffer);}
void glBufferData(GLenum target, GLsizeiptr size, const GLvoid* data, GLenum usage) {g_gles->BufferData(g_context, target, size, data, usage);}
void glBufferSubData(GLenum target, GLintptr offset, GLsizeiptr size, const GLvoid* data) {g_gles->BufferSubData(g_context, target, offset, size, data);}
void glDeleteBuffers(GLsizei n, const GLuint* buffers) {g_gles->DeleteBuffers(g_context, n, buffers);}
void glEnableVertexAttribArray(GLuint index) {g_gles->EnableVertexAttribArray(g_context, index);}
void glDisableVertexAttribArray(GLuint index) {g_gles->DisableVertexAttribArray(g_context, index);}
void glVertexAttribPointer(GLuint indx, GLint size, GLenum type, GLboolean normalized, GLsizei stride, const GLvoid* ptr) {g_gles->VertexAttribPointer(g_context, indx, size, type, normalized, stride, ptr);}
void glBindAttribLocation(GLuint program, GLuint index, const GLchar* name) {g_gles->BindAttribLocation(g_context, program, index, name);}
GLint glGetAttribLocation(GLuint program, const GLchar* name) {g_gles->GetAttribLocation(g_context, program, name);}

// ---------------------------------------------
// SurfaceBuffers
// ---------------------------------------------
void glDepthMask(GLboolean flag) {g_gles->DepthMask(g_context, flag);}
void glDepthFunc(GLenum func) {g_gles->DepthFunc(g_context, func);}
void glStencilFunc(GLenum func, GLint refx, GLuint mask) {g_gles->StencilFunc(g_context, func, refx, mask);}
void glStencilOp(GLenum fail, GLenum zfail, GLenum zpass) {g_gles->StencilOp(g_context, fail, zfail, zpass);}
void glBindTexture(GLenum target, GLuint texture) {g_gles->BindTexture(g_context, target, texture);}
void glGenTextures(GLsizei n, GLuint* textures) {g_gles->GenTextures(g_context, n, textures);}
void glTexImage2D(GLenum target, GLint level, GLint internalformat, GLsizei width, GLsizei height, GLint border, GLenum format, GLenum type, const GLvoid* data) {g_gles->TexImage2D(g_context, target, level, internalformat, width, height, border, format, type, data);}
void glDeleteTextures(GLsizei n, const GLuint* textures) {g_gles->DeleteTextures(g_context, n, textures);}
void glClearColor(GLclampf red, GLclampf green, GLclampf blue, GLclampf alpha) {g_gles->ClearColor(g_context, red, green, blue, alpha);}
void glClear(GLbitfield mask) {g_gles->Clear(g_context, mask);}
void glColorMask(GLboolean red, GLboolean green, GLboolean blue, GLboolean alpha) {g_gles->ColorMask(g_context, red, green, blue, alpha);}
void glBlendFunc(GLenum sfactor, GLenum dfactor) {g_gles->BlendFunc(g_context, sfactor, dfactor);}
void glBlendEquation(GLenum mode) {g_gles->BlendEquation(g_context, mode);}
void glBlendEquationSeparate(GLenum modeRGB, GLenum modeAlpha) {g_gles->BlendEquationSeparate(g_context, modeRGB, modeAlpha);}
void glBlendFuncSeparate(GLenum srcRGB, GLenum dstRGB, GLenum srcAlpha, GLenum dstAlpha) {g_gles->BlendFuncSeparate(g_context, srcRGB, dstRGB, srcAlpha, dstAlpha);}
void glCompressedTexImage2D(GLenum target, GLint level, GLenum internalformat, GLsizei width, GLsizei height, GLint border, GLsizei imageSize, const GLvoid* data) {g_gles->CompressedTexImage2D(g_context, target, level, internalformat, width, height, border, imageSize, data);}
void glGenRenderbuffers(GLsizei n, GLuint* renderbuffers) {g_gles->GenRenderbuffers(g_context, n, renderbuffers);}
void glBindRenderbuffer(GLenum target, GLuint renderbuffer) {g_gles->BindRenderbuffer(g_context, target, renderbuffer);}
void glGenFramebuffers(GLsizei n, GLuint* framebuffers) {g_gles->GenFramebuffers(g_context, n, framebuffers);}
void glBindFramebuffer(GLenum target, GLuint renderbuffer) {g_gles->BindFramebuffer(g_context, target, renderbuffer);}
GLenum glCheckFramebufferStatus(GLenum target) {return g_gles->CheckFramebufferStatus(g_context, target);}
void glDeleteFramebuffers(GLsizei n, const GLuint* framebuffers) {g_gles->DeleteFramebuffers(g_context, n, framebuffers);}
void glRenderbufferStorage(GLenum target, GLenum internalformat, GLsizei width, GLsizei height) {g_gles->RenderbufferStorage(g_context, target, internalformat, width, height);}
void glFramebufferRenderbuffer(GLenum target, GLenum attachment, GLenum renderbuffertarget, GLuint renderbuffer) {g_gles->FramebufferRenderbuffer(g_context, target, attachment, renderbuffertarget, renderbuffer);}
void glDeleteRenderbuffers(GLsizei n, const GLuint* renderbuffers) {g_gles->DeleteRenderbuffers(g_context, n, renderbuffers);}
void glFramebufferTexture2D(GLenum target, GLenum attachment, GLenum textarget, GLuint texture, GLint level) {g_gles->FramebufferTexture2D(g_context, target, attachment, textarget, texture, level);}
void glActiveTexture(GLenum texture) {g_gles->ActiveTexture(g_context, texture);}

// ---------------------------------------------
// General
// ---------------------------------------------
const GLubyte* glGetString(GLenum name) {return g_gles->GetString(g_context, name);}
void glGetIntegerv(GLenum pname, GLint* params) {g_gles->GetIntegerv(g_context, pname, params);}
GLenum glGetError() {return g_gles->GetError(g_context);}
void glEnable(GLenum cap) {g_gles->Enable(g_context, cap);}
void glDisable(GLenum cap) {g_gles->Disable(g_context, cap);}
void glViewport(GLint x, GLint y, GLsizei width, GLsizei height) {g_gles->Viewport(g_context, x, y, width, height);}
void glFinish() {g_gles->Finish(g_context);}
void glFlush() {g_gles->Flush(g_context);}
#pragma endregion