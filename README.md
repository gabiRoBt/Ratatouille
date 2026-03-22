<div align="center">
  <img src="Ratatouille/Resources/Ratatouille.png" width="300" alt="Ratatouille Logo"/>

  <p>A Visual Studio extension that generates code from natural language prompts — typed back into the editor character by character, as if written by hand.</p>
</div>

---

## 🪤 Features

- **Multilingual Prompts:** Write your requests in English, Romanian, Spanish, or any other language.
- **Human-Like Typing:** Code is typed out realistically, helping you follow the logic as it's written.
- **Granular Control:** Pause, stop, or instantly complete the typing process at any moment using quick shortcuts.
- **Powered by Gemini:** Leverage Google's state-of-the-art AI models directly in your IDE.

## 🖱️ Installation

1. Download the latest `.vsix` file from the [Releases](../../releases) page.
2. Double-click the file to install it, then restart Visual Studio.
3. Navigate to **Tools → Options → Ratatouille** and enter your Gemini API key.

> **Note:** You can get a free API key at [Google AI Studio](https://aistudio.google.com).

## 🐭 Usage

Type a prompt in your editor, highlight the text, and press `Ctrl+Alt+Shift+R → R`.

**Example:**

Binary search in python

*The prompt will be cleared, and the extension will immediately begin typing out the generated C# code character by character.*

## 🐁 Shortcuts

All Ratatouille actions begin with the chord `Ctrl+Alt+Shift+R`, followed by a specific key.

| Key | Action | Description |
|-----|--------|-------------|
| `R` | **Run** | Generate code from the currently selected prompt |
| `P` | **Pause / Resume** | Temporarily halt or resume the fake typing |
| `S` | **Stop** | Cancel the current generation process entirely |
| `F` | **Finish** | Skip the typing animation and paste all remaining code instantly |

> **Pro Tip:** To reassign these shortcuts, go to **Tools → Options → Environment → Keyboard** and search for `Ratatouille`.

## 🐀 Status Bar Indicators

Keep an eye on the Visual Studio status bar to see what Ratatouille is currently doing:

| Symbol | Meaning |
|--------|---------|
| `Ratatouille` | Waiting for the AI's response |
| `...` | Generating (updates with one dot every ~0.7s) |
| `!` | Code is ready — fake typing is starting |
| `-` | Generation/Typing is paused |
| `.` | Process is stopped or successfully finished |

## 🐹 Configuration Options

Customize your experience via **Tools → Options → Ratatouille**:

| Option | Description |
|--------|-------------|
| **Gemini API Key** | Your personal key from Google AI Studio. |
| **Gemini Model** | Choose your balance of speed and limits: <br> `flash-lite` (1000 requests/day) <br> `flash` (250 requests/day) <br> `pro` (50 requests/day) |
| **Temperature** | `0.0` (Precise & predictable) → `1.0` (Highly creative). |
| **System Prompt** | Custom instructions sent to the AI before every request (e.g., "Always write comments in English"). |

## 🧀 Requirements

- Visual Studio 2022 (Version 17.0 or higher)
- .NET Framework 4.7.2
- Active internet connection
- [Google AI Studio](https://aistudio.google.com) API key

---
---
---

<div align="center">
<pre>
    ...     ..      ..          ..                     ....                ..      .     
  x*8888x.:*8888: -"888:     :**888H: `: .xH""     .xH888888Hx.         x88f` `..x88. .> 
 X   48888X `8888H  8888    X   `8888k XX888     .H8888888888888:     :8888   xf`*8888%  
X8x.  8888X  8888X  !888>  '8hx  48888 ?8888     888*"""?""*88888X   :8888f .888  `"`    
X8888 X8888  88888   "*8%- '8888 '8888 `8888    'f     d8x.   ^%88k  88888' X8888. >"8x  
'*888!X8888> X8888  xH8>    %888>'8888  8888    '>    <88888X   '?8  88888  ?88888< 888> 
  `?8 `8888  X888X X888>      "8 '888"  8888     `:..:`888888>    8> 88888   "88888 "8%  
  -^  '888"  X888  8888>     .-` X*"    8888            `"*88     X  88888 '  `8888>     
   dx '88~x. !88~  8888>       .xhx.    8888       .xHHhx.."      !  `8888> %  X88!      
 .8888Xf.888x:!    X888X.:   .H88888h.~`8888.>    X88888888hx. ..!    `888X  `~""`   :   
:""888":~"888"     `888*"   .~  `%88!` '888*~    !   "*888888888"       "88k.      .~    
    "~'    "~        ""           `"     ""             ^"***"`           `""*==~~`      
  
</pre>
<pre>
     ...     ..                         
  .=*8888x <"?88h.      .xnnx.  .xx.    
 X>  '8888H> '8888    .f``"888X< `888.  
'88h. `8888   8888    8L   8888X  8888  
'8888 '8888    "88>  X88h. `8888  X888k 
 `888 '8888.xH888x.  '8888 '8888  X8888 
   X" :88*~  `*8888>  `*88>'8888  X8888 
 ~"   !"`      "888>    `! X888~  X8888 
  .H8888h.      ?88    -`  X*"    X8888 
 :"^"88888h.    '!      xH88hx  . X8888 
 ^    "88888hx.+"     .*"*88888~  X888X 
        ^"**""        `    "8%    X888> 
                         .x..     888f  
                        88888    :88f   
                        "88*"  .x8*~    

</pre>
<pre>
        ....        .        ..                   ...     ..                  
   .x88" `^x~  xH(`     :**888H: `: .xH""    .=*8888x <"?88h.         oe    
  X888   x8 ` 8888h    X   `8888k XX888     X>  '8888H> '8888       .@88    
 88888  888.  %8888   '8hx  48888 ?8888    '88h. `8888   8888   ==*88888    
<8888X X8888   X8?    '8888 '8888 `8888    '8888 '8888    "88>     88888    
X8888> 488888>"8888x   %888>'8888  8888     `888 '8888.xH888x.     88888    
X8888>  888888 '8888L    "8 '888"  8888       X" :88*~  `*8888>    88888    
?8888X   ?8888>'8888X   .-` X*"    8888     ~"   !"`      "888>    88888    
 8888X h  8888 '8888~     .xhx.    8888      .H8888h.      ?88     88888    
  ?888  -:8*"  <888"    .H88888h.~`8888.>   :"^"88888h.    '!      88888    
   `*88.      :88%     .~  `%88!` '888*~    ^    "88888hx.+"       88888    
      ^"~====""`             `"     ""             ^"**""       '**%%%%%%** 
                                                                            
</pre>

</div>
