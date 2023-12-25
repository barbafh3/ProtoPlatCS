using Raylib_cs;

var image = Raylib.LoadImage("Assets/ph-character/48x48/jump.png");
Raylib.ExportImageAsCode(image, "PHJump.c");
