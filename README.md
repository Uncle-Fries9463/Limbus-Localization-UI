# Limbus Company Localization Interface

## Currently able to edit:
- `Bufs*.json` and `BattleKeywords*.json`
- `Skills*.json`
- `Passives*.json`
- `EGOgift*.json`

## CONFIG SETUP MANUAL: https://gist.github.com/x1bViolet/86bd93b8ac147fec74e1051850814e85

## Interface navigation:
1. To save any description, press CTRL + S. For names there are special button.
2. To fast id switch, click with right mouse button on ID (LMB is copy), type ID or any Name (Automatically finds an existing one), then press Enter to switch.
3. For jump to end/start of file click with right mouse button to Next/Previous switch buttons.
4. ID switch also can be executed by pressing Left/Right arrows on keyboard or Back/Forward mouse buttons. In skills, you also can hold Shift while pressing Back/Forward mouse buttons to switch between uptie levels.
5. In passives, you can create `"summary"` desc elements for passives by clicking on unhighlighted (Disabled) summary desc switch buttons, tooltip will appear.
6. In skills you can click with right mouse button to coin descriptions on preview for fast switch on them.
7. In `Bufs` you able to edit name in textfield on preview, and 'OK' button saves both name and description.
8. Object name in right menu can be scrolled by drag scroll, same as all limbus previews.

## Files saving behavior
- Program tries to keep original encoding of file on save, otherwise uses UTF8.
- Line break is LF, not CRLF.
- All `null` descs (Non-existing) (Coin descs or regular descs of keywords or skills) being replaced with empty strings.
- **By any of any ways make sure you backup your localization files and after saving file is not corrupted (What should not be).**

## UI Languages, Themes and Configuration
Lanugage settings located at `⇲ Assets Directory\[+] Languages` and being selected in `⇲ Assets Directory\Configurazione.json` as all other settings. Theme being selected by its folder name in `⇲ Assets Directory\[+] Themes`.
Almost all ui elements supports this 'TextMeshPro' with small amount of tags that being used in limbus preview system (E.g. `<color=#abcdef>`, `<i>` or `<font>`), in ui you can use `<sprite name="...">` with id of some Keyword or E.G.O gift by their ID (`⇲ Assets Directory\[⇲] Limbus Images`), they can be adjusted with technical `<spritessize>`, `<spriteshoffset>` and `<spritesvoffset>` tags (Not avalible for limbus) with values as `=+/-integer` (`<spritesvoffset=-11>` or `<spritessize=+66>`).

------
![image](https://github.com/user-attachments/assets/470f4c8f-c49e-4f6f-b03f-77da375af4a9)
![Screenshot 2025-05-14 234127](https://github.com/user-attachments/assets/143e1f52-5403-4e8f-b400-142055be6f91)
![Screenshot 2025-05-14 234043](https://github.com/user-attachments/assets/e7035f73-a9d2-4df4-92a3-4de2093fbfff)
![Screenshot 2025-05-14 234127](https://github.com/user-attachments/assets/32ba8ac2-63d6-4c64-bbfb-32e83e6c0c71)
![image](https://github.com/user-attachments/assets/a23ec7f4-eb26-401d-9abe-45ee13deb4b5)
![image](https://github.com/user-attachments/assets/f9aeddc3-52b0-4697-80c1-91aef65b9f44)
