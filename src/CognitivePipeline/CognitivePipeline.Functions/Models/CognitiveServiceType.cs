using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CognitivePipeline.Functions.Models
{
    public enum CognitiveServiceType
    {
        FaceDetection,
        FaceVerification,
        FaceDetectionBasic,
        OCR,
        ImageAnalysis,
        ObjectDetection,
        CustomVisionClassification,
        CustomVisionObjectDetection
    }
}