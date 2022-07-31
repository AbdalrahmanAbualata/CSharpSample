namespace CSharpSample
{
    internal class Constants
    {
        public const string Title = @"iData Iris SDK Sample (VC#)";
        public const string ProductName = @"iData Iris SDK";
        // Cropping modes
        public const int CROP_TYPE_CROPPED = 400;
        public const int CROP_TYPE_CROP_AND_MASKED = 500;

        // Image Type
        public const int IRIS_IMAGE_RECT = 100;
        public const int IRIS_IMAGE_POLAR_NO_BOUND_POLAR = 200;
        public const int IRIS_IMAGE_POLAR_BOUND = 300;

        //Matching Modes
        public const int MATCHING_MODE_STANDARD = 1;
        public const int MATCHING_MODE_EXPRESS = 2;

        public const int IRIS_IMAGE_HEIGHT = 480;
        public const int IRIS_IMAGE_WIDTH = 640;

        // capture device id(this is user defined)
        public const int DEV_ID_ICAM7000 = 1;
        public const int DEV_ID_TD100 = 2;

        //image transformation
        public const int TRANS_UNDEF = 0;
        public const int TRANS_STD = 1;

        //image properties (this is user defined)
        public const int IMG_PROP_RT_RECT = unchecked(0x0000);
        public const int IMG_PROP_RT_POLAR = unchecked(0x0001);

        //Image formats
        public const int IMAGEFORMAT_MONO_RAW = unchecked(0X0002);
        public const int IMAGEFORMAT_RGB_RAW = unchecked(0X0004);
        public const int IMAGEFORMAT_MONO_JPEG = unchecked(0X0006);
        public const int IMAGEFORMAT_RGB_JPEG = unchecked(0X0008);
        public const int IMAGEFORMAT_MONO_JPEG_LS = unchecked(0X000A);
        public const int IMAGEFORMAT_RGB_JPEG_LS = unchecked(0X000C);
        public const int IMAGEFORMAT_MONO_JPEG2000 = unchecked(0X000E);
        public const int IMAGEFORMAT_RGB_JPEG2000 = unchecked(0X0010);
        public const int IMAGEFORMAT_MONO_BMP = unchecked(0X000B);
        //Image formats for ISO 2011
		public const int ISO2011_IMAGEFORMAT_MONO_RAW =	2;
		public const int ISO2011_IMAGEFORMAT_MONO_JPEG2000 = 10; // 0X0A
		public const int ISO2011_IMAGEFORMAT_MONO_PNG = 14; // 0X0E

        //// ISO Error codes
		public const int IAIRIS_LICENCE_EXPIRED = -2097151994;
		public const int IAIRIS_NOT_MATCHED = -2147414015;
		public const int IAIRIS_ERROR_CONTACT_LENS = -2147418073;
    }

	public enum enmImageFormat
	{
		RAW, JPEG, JPEG2K, PNG, BMP
	};
}