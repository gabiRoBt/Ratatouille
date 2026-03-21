# !\[Ratatouille](Resources/Ratatouille.png)

# 

# > AI-powered code generation for Visual Studio — typed out character by character, as if written by hand.

# 

# !\[Version](https://img.shields.io/badge/version-1.0-blue)

# !\[VS](https://img.shields.io/badge/Visual%20Studio-2022-purple)

# !\[.NET](https://img.shields.io/badge/.NET-4.7.2-green)

# 

# \---

# 

# \## What is it?

# 

# Ratatouille lets you write a plain-English prompt directly in your code file, select it, and press a shortcut. The AI reads your prompt, generates the code, and types it back into the editor — character by character, as if someone is writing it live.

# 

# No side panels. No popups. Just your editor.

# 

# \---

# 

# \## Installation

# 

# 1\. Download the `.vsix` file from the \[Releases](../../releases) page

# 2\. Double-click it to install

# 3\. Restart Visual Studio

# 4\. Go to \*\*Tools → Options → Ratatouille\*\* and enter your Gemini API key

# 

# > You can get a free API key at \[aistudio.google.com](https://aistudio.google.com)

# 

# \---

# 

# \## Usage

# 

# 1\. Type a prompt anywhere in your editor, for example:

# &#x20;  ```

# &#x20;  binary search function in C#

# &#x20;  ```

# 2\. Select the prompt text

# 3\. Press `Ctrl+Alt+Shift+R` → `R`

# 4\. Watch the code appear

# 

# \---

# 

# \## Shortcuts

# 

# All shortcuts start with `Ctrl+Alt+Shift+R`, followed by a second key.

# 

# | Shortcut | Action |

# |---|---|

# | `Ctrl+Alt+Shift+R` → `R` | Generate code from selected prompt |

# | `Ctrl+Alt+Shift+R` → `P` | Pause / Resume fake typing |

# | `Ctrl+Alt+Shift+R` → `S` | Stop fake typing completely |

# | `Ctrl+Alt+Shift+R` → `F` | Finish — paste all remaining code instantly |

# 

# > To reassign any shortcut: \*\*Tools → Options → Environment → Keyboard\*\*, search for `Ratatouille`.

# 

# \---

# 

# \## Status Bar Indicators

# 

# | Symbol | Meaning |

# |---|---|

# | `Ratatouille` | Shortcut pressed, waiting for AI response |

# | `...` | AI is generating (one dot every 0.7s) |

# | `!` | Code received and ready — start typing |

# | `-` | Fake typing is paused |

# | `.` | Stopped or finished |

# 

# \---

# 

# \## Options

# 

# Go to \*\*Tools → Options → Ratatouille\*\* to configure:

# 

# \### 1. API Authentication

# 

# | Option | Description |

# |---|---|

# | \*\*Gemini API Key\*\* | Your secret key from Google AI Studio |

# | \*\*Gemini Model\*\* | Model used for generation (see below) |

# | \*\*Temperature\*\* | Controls creativity: `0.0` = precise, `1.0` = creative |

# 

# \#### Available Models

# 

# | Model | Speed | Free tier | Best for |

# |---|---|---|---|

# | `gemini-2.5-flash-lite` | fastest | 1000 req/day | everyday use ✓ |

# | `gemini-2.5-flash` | fast | 250 req/day | complex prompts |

# | `gemini-2.5-pro` | slower | 50 req/day | hard problems |

# 

# \### 2. Generation

# 

# | Option | Description |

# |---|---|

# | \*\*Custom System Prompt\*\* | Instructions sent to the AI before every request. Controls tone, language, style. Leave empty for default behavior. |

# 

# \*\*Default system prompt:\*\*

# ```

# You are a coding assistant integrated directly into Visual Studio.

# Reply ONLY with the exact code requested, no explanations, no Markdown fences.

# ```

# 

# \### 3. Shortcut

# 

# Read-only field showing the active shortcut. Change it via \*\*Tools → Options → Environment → Keyboard\*\*.

# 

# \---

# 

# \## How it works

# 

# ```

# Select prompt → Ctrl+Alt+Shift+R → R

# &#x20;      ↓

# &#x20; Prompt sent to Gemini API

# &#x20;      ↓

# &#x20; Response received

# &#x20;      ↓

# &#x20; Prompt text deleted from editor

# &#x20;      ↓

# &#x20; Code typed character by character

# &#x20; (each keypress advances one character)

# &#x20;      ↓

# &#x20; 0.7s cooldown after last character

# &#x20; (keys swallowed to prevent accidental input)

# &#x20;      ↓

# &#x20; Done

# ```

# 

# \---

# 

# \## Requirements

# 

# \- Visual Studio 2022 (v17.0 or later)

# \- .NET Framework 4.7.2

# \- A free \[Google AI Studio](https://aistudio.google.com) API key

# 

# \---

# 

# \## Author

# 

# \*\*Gabriel Feraru\*\*

