<template>
  <div class="modern-editor">
    <div v-if="editor" class="toolbar">
      <div class="button-group">
        <button
          @click="editor.chain().focus().toggleBold().run()"
          :class="{ 'is-active': editor.isActive('bold') }"
          title="Bold"
        >
          <span class="icon">B</span>
        </button>
        <button
          @click="editor.chain().focus().toggleItalic().run()"
          :class="{ 'is-active': editor.isActive('italic') }"
          title="Italic"
        >
          <span class="icon">I</span>
        </button>
      </div>

      <div class="divider"></div>

      <div class="button-group">
        <button
          @click="editor.chain().focus().toggleHeading({ level: 1 }).run()"
          :class="{ 'is-active': editor.isActive('heading', { level: 1 }) }"
        >
          H1
        </button>
        <button
          @click="editor.chain().focus().toggleHeading({ level: 2 }).run()"
          :class="{ 'is-active': editor.isActive('heading', { level: 2 }) }"
        >
          H2
        </button>
      </div>

      <div class="divider"></div>

      <div class="button-group">
        <button @click="editor.chain().focus().undo().run()" :disabled="!editor.can().undo()">
          ↶
        </button>
        <button @click="editor.chain().focus().redo().run()" :disabled="!editor.can().redo()">
          ↷
        </button>
      </div>
    </div>

    <div class="content-wrapper">
      <EditorContent :editor="editor" class="editor-content" />
    </div>
  </div>NjllYTA0MWNiMzkyZjBiNWM5MTdjNzVmLmx3bnhlVk9PaWhGd3p3UnZkN0NLNWNzazdHeU0yNEgt
</template>

<script lang="ts" setup>
import { EditorContent, useEditor } from '@tiptap/vue-3';
import StarterKit from '@tiptap/starter-kit';

const editor = useEditor({
  content: "<h3>Modern Tiptap ✨</h3><p>Type something smooth here...</p>",
  extensions: [StarterKit],
  injectCSS: true,
});
</script>

<style lang="scss">
$radius: 16px;
$primary-color: #6366f1; // Modern Indigo
$bg-color: #ffffff;
$border-color: #f1f5f9;
$shadow: 0 4px 6px -1px rgb(0 0 0 / 0.1), 0 2px 4px -2px rgb(0 0 0 / 0.1);

.modern-editor {
  max-width: 800px;
  margin: 2rem auto;
  font-family: 'Inter', sans-serif;
}

.toolbar {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 8px 12px;
  background: $bg-color;
  border: 1px solid $border-color;
  border-radius: $radius $radius 4px 4px; // Top rounded
  box-shadow: $shadow;
  margin-bottom: 2px;

  .button-group {
    display: flex;
    gap: 4px;
  }

  .divider {
    width: 1px;
    height: 20px;
    background: $border-color;
    margin: 0 4px;
  }

  button {
    display: flex;
    align-items: center;
    justify-content: center;
    width: 36px;
    height: 36px;
    border-radius: 10px; // Very curvy
    border: none;
    background: transparent;
    color: #475569;
    font-weight: 600;
    transition: all 0.2s ease;
    cursor: pointer;

    &:hover:not(:disabled) {
      background: #f8fafc;
      color: $primary-color;
    }

    &.is-active {
      background: #eef2ff;
      color: $primary-color;
    }

    &:disabled {
      opacity: 0.3;
      cursor: not-allowed;
    }
  }
}

.content-wrapper {
  background: $bg-color;
  border: 1px solid $border-color;
  border-radius: 4px 4px $radius $radius; // Bottom rounded
  box-shadow: $shadow;
  padding: 1.5rem;
}

.editor-content {
  .ProseMirror {
    outline: none;
    min-height: 200px;
    line-height: 1.6;
    color: #1e293b;

    h1, h2, h3 { margin-top: 1em; color: #0f172a; }
  }
}
</style>