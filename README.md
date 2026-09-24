# Samples

C# sample collection: CaptureManager SDK WPF demos, Flyleaf media player, and OpenCvSharp samples, plus capture ASF assets. Trees cover webcam/screen/RTSP/RTMP capture, FFmpeg/DirectX playback (FlyleafLib), and OpenCvSharp vision apps, with local `Video/*.asf` demo media. Upstream third-party working copies; licenses retained per subtree (see `THIRD_PARTY_NOTICES.md`).

**Source last updated:** 2022-05-22 · **Language:** C# · **Framework:** .NET Framework 4.0–4.8 and .NET 5/6 (Windows) · **Output:** WPF / WinForms sample apps, class libraries, and demo media

## Solution structure

| Project / area | Language | Type | Purpose |
|---|---|---|---|
| CaptureManagerSDK-CSharpDemos | C# (WPF/WinForms) + C++ (RTMP) | Demo solution (~69 projects) | CaptureManager SDK demos: devices, screen/window capture, recording, mixers, IP MJPEG, RTSP client/server, RTMP, EVR viewers (sync + async pairs) |
| Flyleaf-master | C# | Library + samples/tests | FlyleafLib media player (v3.4) for WPF/WinForms; plugins (YouTube-DL, OpenSubtitles, BitSwarm, subtitles); FFmpeg binaries under `FFmpeg/` |
| OpenCVSharp-Samples-master | C# | Sample apps (19 projects) | OpenCvSharp demos: images, filters, video capture, WPF/WinForms hosts, Haar/Fisher faces, cascade training, OCR |
| Video | ASF media | Demo assets | `Capture 1.asf` and `Capture 2.asf` local capture files for demo playback |

## How to open

- CaptureManager demos: open `CaptureManagerSDK-CSharpDemos/CaptureManagerSDK-CSharpDemos.sln` (requires CaptureManager SDK / `CaptureManager.dll` installed or on the probe path used by the demos).
- Flyleaf: open `Flyleaf-master/FlyleafLib.sln` (FFmpeg DLLs are under `Flyleaf-master/FFmpeg/`).
- OpenCvSharp samples: open `OpenCVSharp-Samples-master/OpenCVSharpSamples.sln` (restore OpenCvSharp NuGet packages).

## Requirements

- **CaptureManagerSDK-CSharpDemos:** Visual Studio 2017 to 2022 (solution VisualStudioVersion 15.x), .NET Framework 4.8, CaptureManager SDK runtime/proxy assemblies, Windows desktop (WPF/WinForms). RTMP native project needs a C++ MSVC toolchain if rebuilding `RTMP`.
- **Flyleaf-master:** Visual Studio 2022 or 2026 (solution VisualStudioVersion 17.x), .NET 6 Windows Desktop workload (samples target `net6.0-windows`); library also multi-targets `net5.0-windows` and `net472`. Windows with DirectX; FFmpeg binaries included under `FFmpeg/`.
- **OpenCVSharp-Samples-master:** Visual Studio 2015 to 2022 (solution Visual Studio 14), .NET Framework 4.0, OpenCvSharp NuGet packages (and OpenCV native runtimes as required by the packages used).

## Attribution and provenance

- Flyleaf / FlyleafLib: SuRGeoNix — https://github.com/SuRGeoNix/Flyleaf (LGPL-3.0-or-later).
- OpenCvSharp Samples: upstream OpenCvSharp samples tree (Apache-2.0) — see `OpenCVSharp-Samples-master/LICENSE.md` and `README.md`.
- CaptureManager SDK C# demos: Evgeny Pereguda / CaptureManager SDK sample set, with bundled OpenSSL, zlib, and librtmp under `3rdparty/` and `RTMP/`.
- `Video/*.asf`: local capture demo assets kept with this working copy.

Working copy from my Development folder `Samples`.

## License

Upstream licenses only — do **not** treat this repo as MIT. See `THIRD_PARTY_NOTICES.md`, plus:

- `Flyleaf-master/LICENSE.txt` (LGPL-3.0)
- `OpenCVSharp-Samples-master/LICENSE.md` (Apache-2.0)
- `CaptureManagerSDK-CSharpDemos/3rdparty/openssl-1.1.0/LICENSE.txt`, `RTMP/librtmp/COPYING` (LGPL-2.1), and zlib notices under `3rdparty/zlib/`
