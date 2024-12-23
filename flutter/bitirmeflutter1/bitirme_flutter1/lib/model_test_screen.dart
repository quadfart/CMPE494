import 'dart:typed_data';
import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';
import 'package:image/image.dart' as img;
import 'package:tflite_flutter/tflite_flutter.dart';

void main() {
  runApp(MyApp());
}

class MyApp extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return MaterialApp(
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

  // Run inference on the image
  Future<void> _classifyImage(img.Image image) async {
    if (_interpreter == null) {
      setState(() {
        _result = 'Model not loaded!';
      });
      return;
    }

    // Resize the image to the model's input size
    final inputSize = 224; // Replace with your model's input size (e.g., 224x224)
    final resizedImage = img.copyResize(image, width: inputSize, height: inputSize);

    // Prepare input and output
    var input = _imageToByteListFloat32(resizedImage, inputSize);
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
        _selectedImage = img.decodeImage(imageBytes);
      });
      if (_selectedImage != null) {
        _classifyImage(_selectedImage!);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: Text('Image Classifier')),
      body: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            _selectedImage != null
                ? Image.memory(Uint8List.fromList(img.encodePng(_selectedImage!)))
                : Text('No image selected.'),
            SizedBox(height: 16),
            Text(_result, style: TextStyle(fontSize: 16)),
            SizedBox(height: 16),
            Row(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                ElevatedButton(
                  onPressed: () => _pickImage(ImageSource.camera),
                  child: Text('Take Photo'),
                ),
                SizedBox(width: 16),
                ElevatedButton(
                  onPressed: () => _pickImage(ImageSource.gallery),
                  child: Text('Select from Gallery'),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}
