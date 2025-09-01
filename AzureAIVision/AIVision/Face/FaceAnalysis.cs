using Azure.AI.Vision.Face;

namespace AIVision.Face;

public class FaceAnalysis(FaceClient client)
{
    public void Analyse(String dataPath)
    {
        using var stream = new FileStream(dataPath, FileMode.Open, FileAccess.Read);

        var detectResponse = client.Detect(
            BinaryData.FromStream(stream),
            FaceDetectionModel.Detection03,
            FaceRecognitionModel.Recognition04,
            returnFaceId: false,
            returnFaceAttributes: new[] { FaceAttributeType.Detection03.HeadPose, FaceAttributeType.Detection03.Mask, FaceAttributeType.Recognition04.QualityForRecognition },
            returnFaceLandmarks: true,
            returnRecognitionModel: true,
            faceIdTimeToLive: 120);

        var detectedFaces = detectResponse.Value;
        // By default, there are 27 predefined landmark points. 
        Console.WriteLine($"Detected {detectedFaces.Count} face(s) in the image.");
        foreach (var detectedFace in detectedFaces)
        {
            Console.WriteLine($"Face Rectangle: left={detectedFace.FaceRectangle.Left}, top={detectedFace.FaceRectangle.Top}, width={detectedFace.FaceRectangle.Width}, height={detectedFace.FaceRectangle.Height}");
            Console.WriteLine($"Head pose: pitch={detectedFace.FaceAttributes.HeadPose.Pitch}, roll={detectedFace.FaceAttributes.HeadPose.Roll}, yaw={detectedFace.FaceAttributes.HeadPose.Yaw}");
            Console.WriteLine($"Mask: NoseAndMouthCovered={detectedFace.FaceAttributes.Mask.NoseAndMouthCovered}, Type={detectedFace.FaceAttributes.Mask.Type}");
            Console.WriteLine($"Quality: {detectedFace.FaceAttributes.QualityForRecognition}");
            Console.WriteLine($"Recognition model: {detectedFace.RecognitionModel}");
            Console.WriteLine($"Landmarks: ");

            Console.WriteLine($"    PupilLeft: ({detectedFace.FaceLandmarks.PupilLeft.X}, {detectedFace.FaceLandmarks.PupilLeft.Y})");
            Console.WriteLine($"    PupilRight: ({detectedFace.FaceLandmarks.PupilRight.X}, {detectedFace.FaceLandmarks.PupilRight.Y})");
            Console.WriteLine($"    NoseTip: ({detectedFace.FaceLandmarks.NoseTip.X}, {detectedFace.FaceLandmarks.NoseTip.Y})");
            Console.WriteLine($"    MouthLeft: ({detectedFace.FaceLandmarks.MouthLeft.X}, {detectedFace.FaceLandmarks.MouthLeft.Y})");
            Console.WriteLine($"    MouthRight: ({detectedFace.FaceLandmarks.MouthRight.X}, {detectedFace.FaceLandmarks.MouthRight.Y})");
            Console.WriteLine($"    EyebrowLeftOuter: ({detectedFace.FaceLandmarks.EyebrowLeftOuter.X}, {detectedFace.FaceLandmarks.EyebrowLeftOuter.Y})");
            Console.WriteLine($"    EyebrowLeftInner: ({detectedFace.FaceLandmarks.EyebrowLeftInner.X}, {detectedFace.FaceLandmarks.EyebrowLeftInner.Y})");
            Console.WriteLine($"    EyeLeftOuter: ({detectedFace.FaceLandmarks.EyeLeftOuter.X}, {detectedFace.FaceLandmarks.EyeLeftOuter.Y})");
            Console.WriteLine($"    EyeLeftTop: ({detectedFace.FaceLandmarks.EyeLeftTop.X}, {detectedFace.FaceLandmarks.EyeLeftTop.Y})");
            Console.WriteLine($"    EyeLeftBottom: ({detectedFace.FaceLandmarks.EyeLeftBottom.X}, {detectedFace.FaceLandmarks.EyeLeftBottom.Y})");
            Console.WriteLine($"    EyeLeftInner: ({detectedFace.FaceLandmarks.EyeLeftInner.X}, {detectedFace.FaceLandmarks.EyeLeftInner.Y})");
            Console.WriteLine($"    EyebrowRightInner: ({detectedFace.FaceLandmarks.EyebrowRightInner.X}, {detectedFace.FaceLandmarks.EyebrowRightInner.Y})");
            Console.WriteLine($"    EyebrowRightOuter: ({detectedFace.FaceLandmarks.EyebrowRightOuter.X}, {detectedFace.FaceLandmarks.EyebrowRightOuter.Y})");
            Console.WriteLine($"    EyeRightInner: ({detectedFace.FaceLandmarks.EyeRightInner.X}, {detectedFace.FaceLandmarks.EyeRightInner.Y})");
            Console.WriteLine($"    EyeRightTop: ({detectedFace.FaceLandmarks.EyeRightTop.X}, {detectedFace.FaceLandmarks.EyeRightTop.Y})");
            Console.WriteLine($"    EyeRightBottom: ({detectedFace.FaceLandmarks.EyeRightBottom.X}, {detectedFace.FaceLandmarks.EyeRightBottom.Y})");
            Console.WriteLine($"    EyeRightOuter: ({detectedFace.FaceLandmarks.EyeRightOuter.X}, {detectedFace.FaceLandmarks.EyeRightOuter.Y})");
            Console.WriteLine($"    NoseRootLeft: ({detectedFace.FaceLandmarks.NoseRootLeft.X}, {detectedFace.FaceLandmarks.NoseRootLeft.Y})");
            Console.WriteLine($"    NoseRootRight: ({detectedFace.FaceLandmarks.NoseRootRight.X}, {detectedFace.FaceLandmarks.NoseRootRight.Y})");
            Console.WriteLine($"    NoseLeftAlarTop: ({detectedFace.FaceLandmarks.NoseLeftAlarTop.X}, {detectedFace.FaceLandmarks.NoseLeftAlarTop.Y})");
            Console.WriteLine($"    NoseRightAlarTop: ({detectedFace.FaceLandmarks.NoseRightAlarTop.X}, {detectedFace.FaceLandmarks.NoseRightAlarTop.Y})");
            Console.WriteLine($"    NoseLeftAlarOutTip: ({detectedFace.FaceLandmarks.NoseLeftAlarOutTip.X}, {detectedFace.FaceLandmarks.NoseLeftAlarOutTip.Y})");
            Console.WriteLine($"    NoseRightAlarOutTip: ({detectedFace.FaceLandmarks.NoseRightAlarOutTip.X}, {detectedFace.FaceLandmarks.NoseRightAlarOutTip.Y})");
            Console.WriteLine($"    UpperLipTop: ({detectedFace.FaceLandmarks.UpperLipTop.X}, {detectedFace.FaceLandmarks.UpperLipTop.Y})");
            Console.WriteLine($"    UpperLipBottom: ({detectedFace.FaceLandmarks.UpperLipBottom.X}, {detectedFace.FaceLandmarks.UpperLipBottom.Y})");
            Console.WriteLine($"    UnderLipTop: ({detectedFace.FaceLandmarks.UnderLipTop.X}, {detectedFace.FaceLandmarks.UnderLipTop.Y})");
            Console.WriteLine($"    UnderLipBottom: ({detectedFace.FaceLandmarks.UnderLipBottom.X}, {detectedFace.FaceLandmarks.UnderLipBottom.Y})");
        }
    }
}
