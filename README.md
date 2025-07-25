# 🔤 Unity Autocomplete & Text Suggestion (C#)

A fast and lightweight autocomplete and text suggestion system implemented in pure C# using trie-like node structures — built specifically for Unity projects.

> 💡 No AI. No external libraries. Just clean and deterministic string matching.

---

## ✨ Features

- 🔎 **Autocomplete** for words or phrases as the user types
- 📚 **Dictionary loading** from `.txt` files or in-memory string lists
- ⚡ **Trie-like structure** (custom node graph) for efficient prefix lookup
- 🧠 Supports frequency-based suggestions and custom user bias
- 💬 Ideal for command input, search bars, chat, or in-game naming
- ✅ Runs entirely offline, fast enough for mobile or WebGL

---

## 🚀 Getting Started

### 1. Clone or Import

Copy the `Autocomplete` folder into your Unity project’s `Assets/` directory.

### 2. Prepare Your Word Source

You can load words from a text file:
```csharp
string text = File.ReadAllText(path);
autocomplete.LoadFromText(text);
