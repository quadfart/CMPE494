import 'package:flutter/material.dart';
import 'package:camera/camera.dart';
import 'package:image_picker/image_picker.dart';
import 'process.dart'; // Process ekranını import ettik

class CameraScreen extends StatefulWidget {
  const CameraScreen({Key? key}) : super(key: key);

  @override
  _CameraScreenState createState() => _CameraScreenState();
}

class _CameraScreenState extends State<CameraScreen> {
  CameraController? _cameraController;
  List<CameraDescription>? cameras;
  bool isCameraInitialized = false;
  String? errorMessage;

  @override
  void initState() {
    super.initState();
    initializeCamera();
  }

  Future<void> initializeCamera() async {
    try {
      cameras = await availableCameras();
      if (cameras != null && cameras!.isNotEmpty) {
        _cameraController = CameraController(
          cameras![0],
          ResolutionPreset.high,
        );
        await _cameraController?.initialize();
        setState(() {
          isCameraInitialized = true;
        });
      } else {
        setState(() {
          errorMessage = "No cameras available.";
        });
      }
    } catch (e) {
      setState(() {
        errorMessage = "Error initializing camera: $e";
      });
    }
  }

  @override
  void dispose() {
    _cameraController?.dispose();
    super.dispose();
  }

  void takePhoto(BuildContext context) async {
    if (_cameraController != null && _cameraController!.value.isInitialized) {
      final XFile? photo = await _cameraController?.takePicture();
      if (photo != null) {
        // Fotoğraf çekildikten sonra Process ekranına yönlendiriyoruz
        /* Navigator.push(
          context,
          MaterialPageRoute(
            builder: (context) => ProcessScreen(photoPath: photo.path),
          ),
        ); */
        if (mounted) {
          Navigator.pop(context, photo);
        }
      }
    }
  }

  Future<void> pickFromGallery(BuildContext context) async {
    final ImagePicker picker = ImagePicker();
    final XFile? image = await picker.pickImage(source: ImageSource.gallery);
    if (image != null) {
      Navigator.push(
        context,
        MaterialPageRoute(
          builder: (context) => ProcessScreen(photoPath: image.path),
        ),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("Camera"),
        backgroundColor: Colors.green,
      ),
      body: errorMessage != null
          ? Center(
              child: Text(
                errorMessage!,
                style: const TextStyle(fontSize: 16, color: Colors.red),
                textAlign: TextAlign.center,
              ),
            )
          : isCameraInitialized
              ? Stack(
                  children: [
                    CameraPreview(_cameraController!), // Kamerayı gösteriyoruz
                    Align(
                      alignment: Alignment.bottomCenter,
                      child: Padding(
                        padding: const EdgeInsets.all(20.0),
                        child: Row(
                          mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                          children: [
                            /* FloatingActionButton(
                              onPressed: () => pickFromGallery(context),
                              backgroundColor: Colors.blue,
                              child: const Icon(Icons.photo_library),
                            ), */
                            FloatingActionButton(
                              onPressed: () => takePhoto(context),
                              backgroundColor: Colors.green,
                              child: const Icon(Icons.camera_alt),
                            ),
                          ],
                        ),
                      ),
                    ),
                  ],
                )
              : const Center(
                  child: CircularProgressIndicator(),
                ),
    );
  }
}
