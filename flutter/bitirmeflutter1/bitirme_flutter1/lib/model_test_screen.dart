import 'dart:typed_data';
import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';
import 'package:image/image.dart' as img;
import 'package:tflite_flutter/tflite_flutter.dart';
import 'camera_screen.dart';

void main() {
  runApp(MyApp());
}

class MyApp extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      theme: ThemeData(
        primarySwatch: Colors.green,
        visualDensity: VisualDensity.adaptivePlatformDensity,
      ),
      home: ImageClassifierScreen(),
    );
  }
}

class ImageClassifierScreen extends StatefulWidget {
  @override
  _ImageClassifierScreenState createState() => _ImageClassifierScreenState();
}

class _ImageClassifierScreenState extends State<ImageClassifierScreen> {
  final ImagePicker _picker = ImagePicker();
  Interpreter? _interpreter;
  String _result = 'Select an image to classify';
  img.Image? _selectedImage;

  @override
  void initState() {
    super.initState();
    _loadModel();
  }

  // Load the TFLite model
  Future<void> _loadModel() async {
    try {
      // Initialize the TensorFlow Lite interpreter
      _interpreter = await Interpreter.fromAsset('assets/model.tflite');
      print('Model loaded successfully!');
    } catch (e) {
      print('Error loading model: $e');
    }
  }

  // Preprocess the image to match the input shape expected by the model
  Uint8List _imageToByteListFloat32(img.Image image, int inputSize) {
    var buffer = Float32List(inputSize * inputSize * 3);
    var bufferIndex = 0;
    for (var y = 0; y < inputSize; y++) {
      for (var x = 0; x < inputSize; x++) {
        var pixel = image.getPixel(x, y);
        buffer[bufferIndex++] = (pixel >> 16 & 0xFF) / 255.0; // R
        buffer[bufferIndex++] = (pixel >> 8 & 0xFF) / 255.0;  // G
        buffer[bufferIndex++] = (pixel & 0xFF) / 255.0;       // B
      }
    }
    return buffer.buffer.asUint8List();
  }

  // Resize image to 250x250 pixels
  img.Image _resizeImage(img.Image image, int width, int height) {
    return img.copyResize(image, width: width, height: height);
  }

  // Run inference on the image
  Future<void> _classifyImage(img.Image image) async {
    if (_interpreter == null) {
      setState(() {
        _result = 'Model not loaded!';
      });
      return;
    }

    // Resize the image to 250x250 pixels
    final resizedImage = _resizeImage(image, 250, 250);

    // Prepare input and output
    var input = _imageToByteListFloat32(resizedImage, 250);
    var output = List.filled(1 * 55, 0.0).reshape([1, 55]); // Adjust 55 to the number of classes in your model

    // Run inference
    _interpreter!.run(input, output);

    // Find the predicted class
    final predictedIndex = output[0].indexOf(output[0].reduce((a, b) => a > b ? a : b));

    setState(() {
      _result = 'Predicted Class: $predictedIndex';
    });
  }

  // Pick an image using ImagePicker
  Future<void> _pickImage(ImageSource source) async {
    final pickedFile = await _picker.pickImage(source: source);
    if (pickedFile != null) {
      final imageBytes = await pickedFile.readAsBytes();
      setState(() {
        _selectedImage = _resizeImage(img.decodeImage(imageBytes)!, 250, 250);
      });
      if (_selectedImage != null) {
        _classifyImage(_selectedImage!);
      }
    }
  }

  void _navigateToCameraScreen() {
    Navigator.push(
      context,
      MaterialPageRoute(builder: (context) => CameraScreen()),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Image Classifier'),
        centerTitle: true,
      ),
      body: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            _selectedImage != null
                ? Image.memory(
              Uint8List.fromList(img.encodePng(_selectedImage!)),
              height: 250,
              width: 250,
              fit: BoxFit.cover,
            )
                : Container(
              height: 200,
              decoration: BoxDecoration(
                color: Colors.grey[200],
                borderRadius: BorderRadius.circular(8),
                border: Border.all(color: Colors.grey),
              ),
              child: const Center(
                child: Text(
                  'No image selected.',
                  style: TextStyle(color: Colors.black54),
                ),
              ),
            ),
            const SizedBox(height: 16),
            Text(
              _result,
              style: const TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
              textAlign: TextAlign.center,
            ),
            const SizedBox(height: 16),
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceEvenly,
              children: [
                ElevatedButton.icon(
                  onPressed: _navigateToCameraScreen,
                  icon: const Icon(Icons.camera_alt),
                  label: const Text('Take Photo'),
                  style: ElevatedButton.styleFrom(
                    padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
                  ),
                ),
                ElevatedButton.icon(
                  onPressed: () => _pickImage(ImageSource.gallery),
                  icon: const Icon(Icons.photo_library),
                  label: const Text('Gallery'),
                  style: ElevatedButton.styleFrom(
                    padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}
