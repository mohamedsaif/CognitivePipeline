using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CognitivePipeline.RTC.Models
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