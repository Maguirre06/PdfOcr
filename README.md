# PDF + OCR
<img width="1916" height="1027" alt="image" src="https://github.com/user-attachments/assets/eb5284f5-807f-47ce-8573-31fcd3677034" />

Visor de archivos PDF para Windows con reconocimiento de texto en espanol.

## Generar el instalador

Requisitos para compilar:

- SDK de .NET 8 o posterior.
- Inno Setup 6.

Desde PowerShell:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\Build-Installer.ps1
```

El instalador se genera en `artifacts\installer`. Publica la aplicacion como
autocontenida para Windows x64, crea los accesos directos y registra **PDF +
OCR** entre las aplicaciones capaces de abrir archivos `.pdf`.

Windows 10 y 11 no permiten que un instalador cambie silenciosamente la
aplicacion PDF predeterminada. Al terminar la instalacion se puede abrir la
pagina de Aplicaciones predeterminadas para que el usuario confirme la
seleccion.
