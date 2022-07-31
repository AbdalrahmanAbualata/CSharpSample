using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using iDataIrisAxLib;

namespace CSharpSample
{
    public partial class FrmMain : Form
    {
        private readonly iDataIrisAxLib.IiDataIris _iDataIris = new iDataIrisClass();

        private readonly string _isoDirectory =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                         Constants.ProductName + @"\ISORecords");

        public FrmMain()
        {
            InitializeComponent();
            try
            {
                string version;
                if (0 == _iDataIris.GetVersion(0, out version))
                {
                    this.Text = Constants.Title + @" v" + version;
                }
                cboIsoDeviceId.SelectedIndex = 0;
				cboIsoDeviceId2011.SelectedIndex = 0;
                
            }
            catch (Exception exception)
            {
                //MessageBox.Show(exception.Message, Constants.TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }
        }

        private void LoadImage_Click(object sender, EventArgs e)
        {
            byte[] rawImageBytes;
            int width, height;
            lblImageQuality.Text = string.Empty;
            if(LoadIrisImage(out rawImageBytes, out width, out height) != 0)
				return;

            if (null == rawImageBytes) return;

            picOriginal.Image = ImageHelper.Raw8BitByteArrayToImage(rawImageBytes, width, height);

            int cropTopX = 0;
            int cropTopY = 0;
            int cropImageWidth = 0;
            int cropImageHeight = 0;
            object croppedImage = null;

            int iResult = _iDataIris.CropIrisImage(Constants.CROP_TYPE_CROPPED, rawImageBytes, width, height, out cropTopX,
                                     out cropTopY, out cropImageWidth,
                                     out cropImageHeight, out croppedImage);
			if (iResult != 0)
			{
				if (iResult == Constants.IAIRIS_LICENCE_EXPIRED)
					MessageBox.Show(@"License expired!", Constants.Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
				else
					MessageBox.Show(string.Format(
					@"CropIrisImage failed with error  0x{0:X}", iResult),
					Constants.Title,
					MessageBoxButtons.OK,
					MessageBoxIcon.Information);
				return;
			}

			picCropped.Image = ImageHelper.Raw8BitByteArrayToImage((byte[])croppedImage, cropImageWidth,
																   cropImageHeight);

            int quality;

            IrisImageQualityInfo irisImageQualityInfo;
			iResult = _iDataIris.GetImageQuality(Constants.IRIS_IMAGE_RECT, rawImageBytes, width, height, out quality,
                                       out irisImageQualityInfo);
			if (iResult != 0)
			{
				if (iResult == Constants.IAIRIS_LICENCE_EXPIRED)
					MessageBox.Show(@"License expired!", Constants.Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
				else
					MessageBox.Show(string.Format(
					@"GetImageQuality failed with error  0x{0:X}", iResult),
					Constants.Title,
					MessageBoxButtons.OK,
					MessageBoxIcon.Information);
				//return;
			}
            lblImageQuality.Text += @"Quality Score: " + quality + Environment.NewLine;
            lblImageQuality.Text += @"IrisRadius: " + irisImageQualityInfo.IrisRadius.value + Environment.NewLine;
            lblImageQuality.Text += @"IrisFocus: " + irisImageQualityInfo.IrisFocus.value + Environment.NewLine;
            lblImageQuality.Text += @"IrisVisibility: " + irisImageQualityInfo.IrisVisibility.value +
                                    Environment.NewLine;
            lblImageQuality.Text += @"SNR: " + irisImageQualityInfo.SNR.value + Environment.NewLine;
            lblImageQuality.Text += @"IrisPupilContrast: " + irisImageQualityInfo.IrisPupilContrast.value +
                                    Environment.NewLine;
            lblImageQuality.Text += @"ScleraIrisContrast: " + irisImageQualityInfo.ScleraIrisContrast.value +
                                    Environment.NewLine;
            lblImageQuality.Text += @"Cropped (W x H): " + cropImageWidth + @" x " + cropImageHeight +
                                    Environment.NewLine;
			//To Be uncommented when GetContactLensInformation is supported in the SDK
			/*
			int iContactLensType = int.MinValue;
			int iContactLensScore = int.MinValue;
			iResult = _iDataIris.GetContactLensInformation(Constants.IRIS_IMAGE_RECT, rawImageBytes, width, height, out iContactLensType,
									   out iContactLensScore);
			if (iResult != 0 && Constants.IAIRIS_ERROR_CONTACT_LENS != iResult)
			{
				if (iResult == Constants.IAIRIS_LICENCE_EXPIRED)
					MessageBox.Show(@"License expired!", Constants.Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
				else
					MessageBox.Show(string.Format(
					@"GetContactLensInformation failed with error  0x{0:X}", iResult),
					Constants.Title,
					MessageBoxButtons.OK,
					MessageBoxIcon.Information);
				return;
			}
			lblImageQuality.Text += @"Contact Lens Type: " + iContactLensType + Environment.NewLine;
			lblImageQuality.Text += @"Contact Lens Score: " + iContactLensScore + Environment.NewLine;*/
        }

        private void btnLoadProbe_Click(object sender, EventArgs e)
        {
            byte[] rawImageBytes;
            int width, height;
            if(LoadIrisImage(out rawImageBytes, out width, out height) != 0)
				return;
            picProbe.Image = ImageHelper.Raw8BitByteArrayToImage(rawImageBytes, width, height);

			if (picProbe.Image != null && picGallery.Image != null)
			{
				btnMatch.Enabled = true;
                btnVerifyImage.Enabled = true;
                btnMatchImage.Enabled = true;
                btnVerify.Enabled = true;
				btnBestQuality.Enabled = true;
				btnBestMatchable.Enabled = true;
			}
			else
			{
				btnMatch.Enabled = false;
                btnVerifyImage.Enabled = false;
                btnMatchImage.Enabled = false;
                btnVerify.Enabled = false;
				btnBestQuality.Enabled = false;
				btnBestMatchable.Enabled = false;
			}
        }

        private void btnLoadGallery_Click(object sender, EventArgs e)
        {
            byte[] rawImageBytes;
            int width, height;
			if (LoadIrisImage(out rawImageBytes, out width, out height) != 0)
				return;
            picGallery.Image = ImageHelper.Raw8BitByteArrayToImage(rawImageBytes, width, height);

			if (picProbe.Image != null && picGallery.Image != null)
			{
				btnMatch.Enabled = true;
                btnVerifyImage.Enabled = true;
                btnMatchImage.Enabled = true;
                btnVerify.Enabled = true;
				btnBestQuality.Enabled = true;
				btnBestMatchable.Enabled = true;
              
			}
			else
			{
				btnMatch.Enabled = false;
                btnVerifyImage.Enabled = false;
                btnMatchImage.Enabled = false;
                btnVerify.Enabled = false;
				btnBestQuality.Enabled = false;
				btnBestMatchable.Enabled = false;
               
			}
        }

        private void btnMatch_Click(object sender, EventArgs e)
        {
            int iResult;
			string strMessage = string.Empty;
            int igalleryImageWidth;
            int igalleryImageHeight;

            lblMatchResults.Text = string.Empty;

			byte[] byGalleryImageRaw = ImageHelper.ImageToRaw8BitByteArray(picGallery.Image, out igalleryImageWidth,
                                                                         out igalleryImageHeight);
            int iprobeImageHeight;
            int iprobeImageWidth;
			byte[] byProbeImageRaw = ImageHelper.ImageToRaw8BitByteArray(picProbe.Image, out iprobeImageWidth,
                                                                       out iprobeImageHeight);
			MatchByIrisCode(byGalleryImageRaw, igalleryImageWidth, igalleryImageHeight, byProbeImageRaw, iprobeImageWidth, iprobeImageHeight, out strMessage, out iResult);
			if (iResult != 0 && iResult != Constants.IAIRIS_NOT_MATCHED)
			{
				if (iResult == Constants.IAIRIS_LICENCE_EXPIRED)
					MessageBox.Show(strMessage, Constants.Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
				else
					MessageBox.Show(strMessage,
					Constants.Title,
					MessageBoxButtons.OK,
					MessageBoxIcon.Information);
				return;
			}
				
			lblMatchResults.Text = strMessage;
        }

		private void MatchByIrisCode(byte[] byGalleryImageRaw, int iGalleryImageWidth, int iGalleryImageHeight, byte[] byProbeImageRaw, int iProbeImageWidth, int iProbeImageHeight, out string strMessage, out int iResult)
		{
			strMessage = string.Empty;
			object oProbeIrisCode;
			ImageInfo stProbeImageInfo;
			iResult = _iDataIris.CreateLongIrisCode(Constants.IRIS_IMAGE_RECT, byProbeImageRaw, iProbeImageWidth,
											   iProbeImageHeight, out oProbeIrisCode, out stProbeImageInfo);
			if (iResult != 0)
			{
				if (iResult == Constants.IAIRIS_LICENCE_EXPIRED)
					strMessage = @"License expired!" + Environment.NewLine;
				else
					strMessage = string.Format(
					@"CreateLongIrisCode failed with error  0x{0:X}{1}", iResult, Environment.NewLine);
				return;
			}

			object oGalleryIrisCode;
			ImageInfo stGalleryImageInfo;
			object oGalleryProcessedImage;
			int iGalleryImageQuality = 0;
			iResult = _iDataIris.CreateIrisCode(Constants.IRIS_IMAGE_RECT, byGalleryImageRaw, iGalleryImageWidth,
											   iGalleryImageHeight, out oGalleryIrisCode, out oGalleryProcessedImage,
											   out iGalleryImageQuality, out stGalleryImageInfo);
			if (iResult != 0)
			{
				if (iResult == Constants.IAIRIS_LICENCE_EXPIRED)
					strMessage = @"License expired!" + Environment.NewLine;
				else
					strMessage = string.Format(
					@"CreateIrisCode failed with error  0x{0:X}{1}", iResult, Environment.NewLine);
				return;
			}

            int matchedIndex = 0;
            float hd;
			iResult = _iDataIris.MatchByLongIrisCode(Constants.MATCHING_MODE_STANDARD, oProbeIrisCode, oGalleryIrisCode,
                                                1,
                                                0.32f, ref matchedIndex, out hd);
			if (iResult != 0 && iResult != Constants.IAIRIS_NOT_MATCHED)
			{
				if (iResult == Constants.IAIRIS_LICENCE_EXPIRED)
					strMessage = @"License expired!" + Environment.NewLine;
				else
					strMessage = string.Format(
					@"MatchByLongIrisCode failed with error  0x{0:X}{1}", iResult, Environment.NewLine);
				return;
			}

			strMessage += @"Matched Index:  " + matchedIndex + Environment.NewLine;
            strMessage += @"Hamming Distance:  " + hd.ToString("0.00") + Environment.NewLine;

            if (matchedIndex < 0)
            {
				strMessage += @"MATCH:  FAILED" + Environment.NewLine;
            }
            else
            {
				strMessage += @"MATCH:  SUCCESSFUL" + Environment.NewLine;
            }
        }

        private void btnIsoRightEye_Click(object sender, EventArgs e)
        {
            byte[] rawImageBytes;
            int width, height;
            lblIsoRecordInfo.Text = string.Empty;
            if(LoadIrisImage(out rawImageBytes, out width, out height) != 0)
				return;
            picIsoRight.Image = ImageHelper.Raw8BitByteArrayToImage(rawImageBytes, width, height);
        }

        private void btnIsoLeftEye_Click(object sender, EventArgs e)
        {
            byte[] rawImageBytes;
            int width, height;
            lblIsoRecordInfo.Text = string.Empty;
            if(LoadIrisImage(out rawImageBytes, out width, out height) != 0)
				return;
            picIsoLeft.Image = ImageHelper.Raw8BitByteArrayToImage(rawImageBytes, width, height);
			btnCreateIso.Enabled = true;
        }

        private void btnCreateIso_Click(object sender, EventArgs e)
        {
            int iResult;

            //btnIsoClear.PerformClick();

            int captureDeviceId = cboIsoDeviceId.SelectedIndex + 1;
            int imageProperties = Constants.IMG_PROP_RT_RECT;
            int irisDiameter = 200; //TODO
            int imageFormat = Constants.IMAGEFORMAT_MONO_RAW; //TODO
            int imageTransform = Constants.TRANS_UNDEF;
            int imageWidth = 640;
            int imageHeight = 480;
            string deviceUid = txtIsoDeviceUID.Text;

            int nSubtypes = 0;
            int nRightImages = 0;
            int nLeftImages = 0;
            int nUndefImages = 0;
            int[] rightImageQuality = new int[1];
            int[] leftImageQuality = new int[1];
            int[] undefImageQuality = new int[1];
            int[] rightImageSize = new int[1];
            int[] leftImageSize = new int[1];
            int[] undefImageSize = new int[1];
            byte[] rightImages = null;
            byte[] leftImages = null;
            byte[] undefImages = null;
            int rightWidth = 0;
            int rightHeight = 0;
            int leftWidth = 0;
            int leftHeight = 0;

            if (picIsoRight.Image != null)
            {
                nSubtypes++;
                nRightImages++;
                rightImages = ImageHelper.ImageToRaw8BitByteArray(picIsoRight.Image, out rightWidth, out rightHeight);
                rightImageSize[0] = rightImages.Length;
				rightImageQuality[0] = 88; //sample data
            }

            if (picIsoLeft.Image != null)
            {
                nSubtypes++;
                nLeftImages++;
                leftImages = ImageHelper.ImageToRaw8BitByteArray(picIsoLeft.Image, out leftWidth, out leftHeight);
                leftImageSize[0] = leftImages.Length;
				leftImageQuality[0] = 91; //sample data
            }

			if (rightWidth != leftWidth ||
				rightHeight != leftHeight)
			{
				//ISO2005 record requires images of the same size!!!!
				MessageBox.Show(
					@"Supplied images are not of the same size. ISO2005 record requires images of the same size!",
					Constants.Title,
					MessageBoxButtons.OK,
					MessageBoxIcon.Information);
				return;
			}

            object isoRecord;
            int isoRecordSize;
			imageWidth = rightWidth;
			imageHeight = rightHeight;
            if (0 !=
                (iResult =
                 _iDataIris.CreateISORecord(captureDeviceId, imageProperties, irisDiameter, imageFormat, imageWidth,
                                            imageHeight, nSubtypes, imageTransform, deviceUid, nRightImages,
                                            nLeftImages, nUndefImages, rightImageQuality, leftImageQuality,
                                            undefImageQuality, rightImageSize, rightImages, leftImageSize,
                                            leftImages, undefImageSize, undefImages, out isoRecord,
                                            out isoRecordSize)))
			{
				if (iResult == Constants.IAIRIS_LICENCE_EXPIRED)
					MessageBox.Show(@"License expired!", Constants.Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
				else
					MessageBox.Show(string.Format(
					@"CreateISORecord failed with error  0x{0:X}", iResult),
					Constants.Title,
					MessageBoxButtons.OK,
					MessageBoxIcon.Information);
				return;
			}

            string deviceUID;
            int intensityDepth;
            if (0 !=
                (iResult =
                 _iDataIris.GetISORecordInfo(isoRecordSize, isoRecord, out captureDeviceId,
                                             out imageProperties,
                                             out irisDiameter, out imageFormat, out imageWidth, out imageHeight,
                                             out nSubtypes, out nRightImages, out nLeftImages, out nUndefImages,
                                             out imageTransform, out deviceUID, out intensityDepth)))
			{
				if (iResult == Constants.IAIRIS_LICENCE_EXPIRED)
					MessageBox.Show(@"License expired!", Constants.Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
				else
					MessageBox.Show(string.Format(
					@"GetISORecordInfo failed with error  0x{0:X}", iResult),
					Constants.Title,
					MessageBoxButtons.OK,
					MessageBoxIcon.Information);
				return;
			}
			/*
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Capture device id: " + captureDeviceId);
            sb.AppendLine("Device UID: " + deviceUID);
            sb.AppendLine("Properties: " + imageProperties);
            sb.AppendLine("Iris diameter: " + irisDiameter);
            sb.AppendLine("Image format: " + imageFormat);
            sb.AppendLine("Image width: " + imageWidth);
            sb.AppendLine("Image height: " + imageHeight);
            sb.AppendLine("Subtypes: " + nSubtypes);
            sb.AppendLine("Right images: " + nRightImages);
            sb.AppendLine("Left images: " + nLeftImages);
            sb.AppendLine("Undef images: " + nUndefImages);
            sb.AppendLine("Transform: " + imageTransform);
            sb.AppendLine("Intensity depth: " + intensityDepth);

            lblIsoRecordInfo.Text = sb.ToString();
			*/
            string fileName = "ISO_"+Guid.NewGuid() + @".isorec";
            if (!ByteArrayToFile(_isoDirectory, fileName, (byte[])isoRecord))
            {
                MessageBox.Show(
                    @"Failed to save ISO record  " + Environment.NewLine + _isoDirectory,
                    Constants.Title,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            MessageBox.Show(String.Format(@"ISO iris record saved successfully as {0}{1}\{2}", Environment.NewLine, _isoDirectory, fileName),
                Constants.Title,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void btnLoadClear_Click(object sender, EventArgs e)
        {
			if(((Button)sender).Name != btnLoadRAWCompress.Name)
            picOriginal.Image = null;
            picCropped.Image = null;
            lblImageQuality.Text = string.Empty;
        }

        private void btnMatchClear_Click(object sender, EventArgs e)
        {
            picProbe.Image = null;
            picGallery.Image = null;
            lblMatchResults.Text = string.Empty;
            btnMatch.Enabled = false;
            btnMatchImage.Enabled = false;
            btnVerify.Enabled = false;
            btnVerifyImage.Enabled = false;
			btnBestQuality.Enabled = false;
			btnBestMatchable.Enabled = false;
        }

        private void btnReadIso_Click(object sender, EventArgs e)
        {
            int iResult;

            OpenFileDialog openFileDialog1 = new OpenFileDialog
                                                 {
                                                     Filter = @"ISOIris Record(*.isorec)|*.isorec",
                                                     InitialDirectory = _isoDirectory
                                                 };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                btnIsoClear.PerformClick();
                byte[] isoIrisRecord = File.ReadAllBytes(openFileDialog1.FileName);

				uint VersionNumber;
				if (0 !=
					(iResult =
					 _iDataIris.GetISORecordVersionNumber(isoIrisRecord, isoIrisRecord.Length, out VersionNumber)))
				{
					if (iResult == Constants.IAIRIS_LICENCE_EXPIRED)
						MessageBox.Show(@"License expired!", Constants.Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
					else
						MessageBox.Show(string.Format(
						@"GetISORecordVersionNumber failed with error  0x{0:X}", iResult),
						Constants.Title,
						MessageBoxButtons.OK,
						MessageBoxIcon.Information);
					return;
				}

				StringBuilder sb = new StringBuilder();
				sb.AppendLine("ISO record version: " + VersionNumber);

                int captureDeviceId;
                int properties;
                int irisDiameter;
                int imageFormat;
                int imageWidth;
                int imageHeight;
                int nSubTypes;
                int nRightImages;
                int nLeftImages;
                int nUndefImages;
                int transform;
                int intensityDepth;
                string deviceUID;
                if (0 !=
                    (iResult =
                     _iDataIris.GetISORecordInfo(isoIrisRecord.Length, isoIrisRecord, out captureDeviceId,
                                                 out properties,
                                                 out irisDiameter, out imageFormat, out imageWidth, out imageHeight,
                                                 out nSubTypes, out nRightImages, out nLeftImages, out nUndefImages,
                                                 out transform, out deviceUID, out intensityDepth)))
				{
					if (iResult == Constants.IAIRIS_LICENCE_EXPIRED)
						MessageBox.Show(@"License expired!", Constants.Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
					else
						MessageBox.Show(string.Format(
						@"GetISORecordInfo failed with error  0x{0:X}", iResult),
						Constants.Title,
						MessageBoxButtons.OK,
						MessageBoxIcon.Information);
					return;
				}

                sb.AppendLine("Capture device id: " + captureDeviceId);
                sb.AppendLine("Device UID: " + deviceUID);
                sb.AppendLine("Properties: " + properties);
                sb.AppendLine("Iris diameter: " + irisDiameter);
                sb.AppendLine("Image format: " + imageFormat);
                sb.AppendLine("Image width: " + imageWidth);
                sb.AppendLine("Image height: " + imageHeight);
                sb.AppendLine("Subtypes: " + nSubTypes);
                sb.AppendLine("Right images: " + nRightImages);
                sb.AppendLine("Left images: " + nLeftImages);
                sb.AppendLine("Undef images: " + nUndefImages);
                sb.AppendLine("Transform: " + transform);
                sb.AppendLine("Intensity depth: " + intensityDepth);

                //Try to display the images
                if (nSubTypes > 0)
                {
                    int imageQuality;
                    int imageSize;
                    object image;
                    if (nRightImages > 0)
                    {
						iResult = _iDataIris.GetImageFromISORecord(isoIrisRecord.Length, isoIrisRecord, 1, 1, out imageQuality,
                                                         out imageSize, out image);
						if (iResult != 0)
						{
							if (iResult == Constants.IAIRIS_LICENCE_EXPIRED)
								MessageBox.Show(@"License expired!", Constants.Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
							else
								MessageBox.Show(string.Format(
								@"GetImageFromISORecord failed for right eye with error  0x{0:X}", iResult),
								Constants.Title,
								MessageBoxButtons.OK,
								MessageBoxIcon.Information);
							return;
						}
						else
						{
							sb.AppendLine("Image quality of right image #1: " + imageQuality);
							picIsoRight.Image = ImageHelper.Raw8BitByteArrayToImage((byte[])image, imageWidth,
																				imageHeight);
						}
                    }

                    if (nLeftImages > 0)
                    {
                        _iDataIris.GetImageFromISORecord(isoIrisRecord.Length, isoIrisRecord, 2, 1, out imageQuality,
                                                         out imageSize, out image);
						if (iResult != 0)
						{
							if (iResult == Constants.IAIRIS_LICENCE_EXPIRED)
								MessageBox.Show(@"License expired!", Constants.Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
							else
								MessageBox.Show(string.Format(
								@"GetImageFromISORecord failed for left eye with error  0x{0:X}", iResult),
								Constants.Title,
								MessageBoxButtons.OK,
								MessageBoxIcon.Information);
							return;
						}
						else
						{
							sb.AppendLine("Image quality of left image #1: " + imageQuality);
							picIsoLeft.Image = ImageHelper.Raw8BitByteArrayToImage((byte[])image, imageWidth,
																			   imageHeight);
						}
                    }
                }
                lblIsoRecordInfo.Text = sb.ToString();
				btnCreateIso.Enabled = false;
            }
        }

        private void btnIsoClear_Click(object sender, EventArgs e)
        {
            picIsoRight.Image = null;
            picIsoLeft.Image = null;
            lblIsoRecordInfo.Text = string.Empty;
			btnCreateIso.Enabled = false;
        }

        private bool ByteArrayToFile(string directoryName, string fileName, byte[] data)
        {
            try
            {
                string completePath = Path.Combine(directoryName, fileName);

                //Create directory if not present
                Directory.CreateDirectory(directoryName);
                using (
                    FileStream fileStream = new FileStream(completePath, FileMode.Create,
                                                           FileAccess.Write))
                {
                    fileStream.Write(data, 0, data.Length);
                    fileStream.Close();
                }

                return true;
            }
            catch (Exception exception)
            {
                MessageBox.Show(@"Failed to save file:" + fileName + Environment.NewLine + exception.Message,
                                Constants.Title, MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
            return false;
        }

		private int ConvertImageToRaw(enmImageFormat ImageFormat, byte[] loadedImageBuffer, out byte[] rawImageBytes, out int rawWidth, out int rawHeight)
		{
            int iResult = 0;
			rawImageBytes = null;
			rawWidth = 0;
			rawHeight = 0;
			object tmpImage;
			int width, height;

			if (ImageFormat == enmImageFormat.JPEG)
			{
				if (0 ==
					(iResult =
					 _iDataIris.JpegToRaw(loadedImageBuffer, loadedImageBuffer.Length, out tmpImage, out width, out height)))
				{
					rawImageBytes = tmpImage as byte[];
					rawWidth = width;
					rawHeight = height;
				}
				else
				{
					if (iResult == Constants.IAIRIS_LICENCE_EXPIRED)
						MessageBox.Show(@"License expired!", Constants.Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
					else
						MessageBox.Show(string.Format(
						@"JpegToRaw failed for left eye with error  0x{0:X}", iResult),
						Constants.Title,
						MessageBoxButtons.OK,
						MessageBoxIcon.Information);
					return -1;
				}
			}
			else if (ImageFormat == enmImageFormat.JPEG2K)
			{
				if (0 ==
					(iResult =
					 _iDataIris.Jpeg2kToRaw(loadedImageBuffer, loadedImageBuffer.Length, out tmpImage, out width, out height)))
				{
					rawImageBytes = tmpImage as byte[];
					rawWidth = width;
					rawHeight = height;
				}
				else
				{
					if (iResult == Constants.IAIRIS_LICENCE_EXPIRED)
						MessageBox.Show(@"License expired!", Constants.Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
					else
						MessageBox.Show(string.Format(
						@"Jpeg2kToRaw failed for left eye with error  0x{0:X}", iResult),
						Constants.Title,
						MessageBoxButtons.OK,
						MessageBoxIcon.Information);
					return -1;
				}
			}
			else if (ImageFormat == enmImageFormat.PNG)
			{
				if (0 ==
					(iResult =
					 _iDataIris.PngToRaw(loadedImageBuffer, loadedImageBuffer.Length, out tmpImage, out width, out height)))
				{
					rawImageBytes = tmpImage as byte[];
					rawWidth = width;
					rawHeight = height;
				}
				else
				{
					if (iResult == Constants.IAIRIS_LICENCE_EXPIRED)
						MessageBox.Show(@"License expired!", Constants.Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
					else
						MessageBox.Show(string.Format(
							@"PngToRaw failed for left eye with error  0x{0:X}", iResult),
							Constants.Title,
							MessageBoxButtons.OK,
							MessageBoxIcon.Information);
					return -1;
				}
			}
                   

			else
				return -1;
			return 0;
		}

        private int LoadIrisImage(out byte[] rawImageBytes, out int rawWidth, out int rawHeight)
        {
            rawImageBytes = null;
            rawWidth = 640;
            rawHeight = 480;
            object bmpImageBytes;
            int bmpImageSize;

            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Filter =
            @"Jpeg/Jpg files (*.jpeg/jpg)|*.jpeg;*.jpg|Jpeg2000 files (*.jp2)|*.jp2|Png (*.png)|*.png|Bitmap Files (*.bmp;*.dib)|*.bmp";
            openFileDialog1.RestoreDirectory = true;

			int iResult = 0;
			if (openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				byte[] loadedImageBuffer = File.ReadAllBytes(openFileDialog1.FileName);

				object tmpImage;
				int width, height;

				if (openFileDialog1.FilterIndex == 1) //JPEG
				{
					if (0 ==
						(iResult =
						 _iDataIris.JpegToRaw(loadedImageBuffer, loadedImageBuffer.Length, out tmpImage, out width,
											  out height)))
					{
						rawImageBytes = tmpImage as byte[];
						rawWidth = width;
						rawHeight = height;
					}
					else
					{
						if (iResult == Constants.IAIRIS_LICENCE_EXPIRED)
							MessageBox.Show(@"License expired!", Constants.Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
						else
							MessageBox.Show(string.Format(
							@"JpegToRaw failed for left eye with error  0x{0:X}", iResult),
							Constants.Title,
							MessageBoxButtons.OK,
							MessageBoxIcon.Information);
						return -1;
					}
				}

				if (openFileDialog1.FilterIndex == 2) //JPEG2000 File
				{
					if (0 ==
						(iResult =
						 _iDataIris.Jpeg2kToRaw(loadedImageBuffer, loadedImageBuffer.Length, out tmpImage, out width,
												out height)))
					{
						rawImageBytes = tmpImage as byte[];
						rawWidth = width;
						rawHeight = height;
					}
					else
					{
						if (iResult == Constants.IAIRIS_LICENCE_EXPIRED)
							MessageBox.Show(@"License expired!", Constants.Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
						else
							MessageBox.Show(string.Format(
							@"Jpeg2kToRaw failed for left eye with error  0x{0:X}", iResult),
							Constants.Title,
							MessageBoxButtons.OK,
							MessageBoxIcon.Information);
						return -1;
					}
				}

				if (openFileDialog1.FilterIndex == 3) //Png File
				{
					if (0 ==
						(iResult =
						 _iDataIris.PngToRaw(loadedImageBuffer, loadedImageBuffer.Length, out tmpImage, out width,
											 out height)))
					{
						rawImageBytes = tmpImage as byte[];
						rawWidth = width;
						rawHeight = height;
					}
					else
					{
						if (iResult == Constants.IAIRIS_LICENCE_EXPIRED)
							MessageBox.Show(@"License expired!", Constants.Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
						else
							MessageBox.Show(string.Format(
								@"PngToRaw failed for left eye with error  0x{0:X}", iResult),
								Constants.Title,
								MessageBoxButtons.OK,
								MessageBoxIcon.Information);
						return -1;
					}
				}
                if (openFileDialog1.FilterIndex == 4) //Bmp File
                {
                    if (0 ==
                        (iResult =
                         _iDataIris.BmpToRaw(loadedImageBuffer, loadedImageBuffer.Length, out tmpImage, out width,
                                             out height)))
                    {
                        rawImageBytes = tmpImage as byte[];
                        rawWidth = width;
                        rawHeight = height;
                        
                    }
                    else
                    {
                        if (iResult == Constants.IAIRIS_LICENCE_EXPIRED)
                            MessageBox.Show(@"License expired!", Constants.Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        else
                            MessageBox.Show(string.Format(
                                @"BmpToRaw failed for left eye with error  0x{0:X}", iResult),
                                Constants.Title,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                        return -1;
                    }
                }
			}
			else
				return -1;
			return 0;
        }

		private void btnBestQuality_Click(object sender, EventArgs e)
		{
			int iResult = 0;
			int galleryImageWidth;
			int galleryImageHeight;

			lblMatchResults.Text = string.Empty;

			byte[] rawGalleryImage = ImageHelper.ImageToRaw8BitByteArray(picGallery.Image, out galleryImageWidth,
																		 out galleryImageHeight);
			int probeImageHeight;
			int probeImageWidth;
			byte[] rawProbeImage = ImageHelper.ImageToRaw8BitByteArray(picProbe.Image, out probeImageWidth,
																	   out probeImageHeight);
			Int32 ilen = (galleryImageWidth * galleryImageHeight) + (probeImageWidth * probeImageHeight);
			byte[] rawImages = new byte[ilen];
			for (int i = 0, x = 0; i < ilen; i++)
			{
				if (i < galleryImageWidth * galleryImageHeight)
				{
					rawImages[i] = rawGalleryImage[i];
					continue;
				}
				rawImages[i] = rawProbeImage[x++];
			}
			int[] iImgWidth = new int[2];
			iImgWidth[0] = galleryImageWidth;
			iImgWidth[1] = probeImageWidth;

			int[] iImgHeight = new int[2];
			iImgHeight[0] = galleryImageHeight;
			iImgHeight[1] = probeImageHeight;

			object BestIrisCode = 0;
		
			int iBestImageQuality = 0;
			IrisImage[] irisImage = new IrisImage[2];

			GCHandle pinnedArrayGallery = GCHandle.Alloc(rawGalleryImage, GCHandleType.Pinned);
			irisImage[0] = new IrisImage();
			irisImage[0].pucIrisImage = pinnedArrayGallery.AddrOfPinnedObject();
			irisImage[0].lImageWidth = galleryImageWidth;
			irisImage[0].lImageHeight = galleryImageHeight;

			GCHandle pinnedArrayProbe = GCHandle.Alloc(rawProbeImage, GCHandleType.Pinned);
			irisImage[1] = new IrisImage();
			irisImage[1].pucIrisImage = pinnedArrayProbe.AddrOfPinnedObject();
			irisImage[1].lImageWidth = probeImageWidth;
			irisImage[1].lImageHeight = probeImageHeight;

            //Pass 0 to BestImageIndex parameter to get the IrisCode.
            int iBestImageIndex = -1;   
			iResult = _iDataIris.SelectBestQualityImage(2, irisImage, out iBestImageIndex, out iBestImageQuality, out BestIrisCode);
			pinnedArrayGallery.Free();
			pinnedArrayProbe.Free();
			if (iResult != 0)
			{
				if (iResult == Constants.IAIRIS_LICENCE_EXPIRED)
					MessageBox.Show(@"License expired!", Constants.Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
				else
					MessageBox.Show(string.Format(
					@"SelectBestQualityImage failed with error  0x{0:X}", iResult),
					Constants.Title,
					MessageBoxButtons.OK,
					MessageBoxIcon.Information);
				return;
			}

			lblMatchResults.Text += @"BestImage Index:  " + iBestImageIndex + Environment.NewLine;
			lblMatchResults.Text += @"Best Image Quality:  " + iBestImageQuality + Environment.NewLine;
			lblMatchResults.Text += @"SelectBestQualityImage:  SUCCESSFULL";
		}

		private void btnBestMatchable_Click(object sender, EventArgs e)
		{
			int iResult = 0;
			int galleryImageWidth;
			int galleryImageHeight;

			lblMatchResults.Text = string.Empty;

			byte[] rawGalleryImage = ImageHelper.ImageToRaw8BitByteArray(picGallery.Image, out galleryImageWidth,
																		 out galleryImageHeight);
			int probeImageHeight;
			int probeImageWidth;
			byte[] rawProbeImage = ImageHelper.ImageToRaw8BitByteArray(picProbe.Image, out probeImageWidth,
																	   out probeImageHeight);
			Int32 ilen = (galleryImageWidth * galleryImageHeight) + (probeImageWidth * probeImageHeight);
			byte[] rawImages = new byte[ilen];
			for (int i = 0, x = 0; i < ilen; i++)
			{
				if (i < galleryImageWidth * galleryImageHeight)
				{
					rawImages[i] = rawGalleryImage[i];
					continue;
				}
				rawImages[i] = rawProbeImage[x++];
			}
			int[] iImgWidth = new int[2];
			iImgWidth[0] = galleryImageWidth;
			iImgWidth[1] = probeImageWidth;

			int[] iImgHeight = new int[2];
			iImgHeight[0] = galleryImageHeight;
			iImgHeight[1] = probeImageHeight;

			object BestIrisCode;
			int iBestImageQuality;
			IrisImage[] irisImage = new IrisImage[2];

			GCHandle pinnedArrayGallery = GCHandle.Alloc(rawGalleryImage, GCHandleType.Pinned);
			irisImage[0] = new IrisImage();
			irisImage[0].pucIrisImage = pinnedArrayGallery.AddrOfPinnedObject();
			irisImage[0].lImageWidth = galleryImageWidth;
			irisImage[0].lImageHeight = galleryImageHeight;

			GCHandle pinnedArrayProbe = GCHandle.Alloc(rawProbeImage, GCHandleType.Pinned);
			irisImage[1] = new IrisImage();
			irisImage[1].pucIrisImage = pinnedArrayProbe.AddrOfPinnedObject();
			irisImage[1].lImageWidth = probeImageWidth;
			irisImage[1].lImageHeight = probeImageHeight;
            
            //Pass 0 to BestImageIndex parameter to get the IrisCode.
            int iBestImageIndex = -1;   
            iResult = _iDataIris.SelectBestMatchableImage(2, irisImage, out iBestImageIndex, out iBestImageQuality, out BestIrisCode);
			pinnedArrayGallery.Free();
			pinnedArrayProbe.Free();

			if (iResult != 0)
			{
				if (iResult == Constants.IAIRIS_LICENCE_EXPIRED)
					MessageBox.Show(@"License expired!", Constants.Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
				else
					MessageBox.Show(string.Format(
					@"SelectBestMatchableImage failed with error  0x{0:X}", iResult),
					Constants.Title,
					MessageBoxButtons.OK,
					MessageBoxIcon.Information);
				return;
			}

			lblMatchResults.Text += @"BestImage Index:  " + iBestImageIndex + Environment.NewLine;
			lblMatchResults.Text += @"Best Image Quality:  " + iBestImageQuality + Environment.NewLine;
			lblMatchResults.Text += @"SelectBestMatchableImage:  SUCCESSFULL";
		}

		private void btnRight2011_Click(object sender, EventArgs e)
		{
			byte[] rawImageBytes;
			int width, height;
			lblIso2011RecordInfo.Text = string.Empty;
			if (LoadIrisImage(out rawImageBytes, out width, out height) != 0)
				return;
			picIso2011Right.Image = ImageHelper.Raw8BitByteArrayToImage(rawImageBytes, width, height);
		}

		private void btnLeft2011_Click(object sender, EventArgs e)
		{
			byte[] rawImageBytes;
			int width, height;
			lblIso2011RecordInfo.Text = string.Empty;
			if (LoadIrisImage(out rawImageBytes, out width, out height) != 0)
				return;
			picIso2011Left.Image = ImageHelper.Raw8BitByteArrayToImage(rawImageBytes, width, height);
			btnCreateISO2011.Enabled = true;
		}

		private void btnCreateISO2011_Click(object sender, EventArgs e)
		{
			int iResult = 0;
			byte byNumberOfEyeRepresented = 0;
			ushort usIrisRepresentationCount = 0;
			byte byCertificationScheme = 0;

			byte[] rightImages = null;
			byte[] leftImages = null;
			int rightWidth = 0, leftWidth = 0, rightHeight = 0, leftHeight = 0;

			ISO2011_IRIS_REPRESENTATION[]	l_pstIrisRepresentation = new ISO2011_IRIS_REPRESENTATION[2];

			if (picIso2011Right.Image != null)
			{
				rightImages = ImageHelper.ImageToRaw8BitByteArray(picIso2011Right.Image, out rightWidth, out rightHeight);
				byNumberOfEyeRepresented = 1;
				usIrisRepresentationCount = 1;
			}
			if (picIso2011Left.Image != null)
			{
				leftImages = ImageHelper.ImageToRaw8BitByteArray(picIso2011Left.Image, out leftWidth, out leftHeight);
				++byNumberOfEyeRepresented;
				++usIrisRepresentationCount;
			}

			int index = 0;

			GCHandle pinnedArrayRight = GCHandle.Alloc(rightImages, GCHandleType.Pinned);
			GCHandle pinnedArrayLeft = GCHandle.Alloc(leftImages, GCHandleType.Pinned);
			if (picIso2011Right.Image != null)
			{
				l_pstIrisRepresentation[index] = new ISO2011_IRIS_REPRESENTATION();
				l_pstIrisRepresentation[index].CaptureDeviceTechID = 1;
				l_pstIrisRepresentation[index].CaptureDeviceVendorID = 11;
				l_pstIrisRepresentation[index].CaptureDeviceTypeID = Convert.ToUInt16(cboIsoDeviceId.SelectedIndex + 1);
				l_pstIrisRepresentation[index].NumberOfQualityBlocks = 0;
				//l_pstIrisRepresentation.QualityRecords;
				l_pstIrisRepresentation[index].EyeLabel = 1;
				l_pstIrisRepresentation[index].ImageType = 1;
				l_pstIrisRepresentation[index].ImageFormat = Constants.IMAGEFORMAT_MONO_RAW;
				l_pstIrisRepresentation[index].ImageProperties = Constants.IMG_PROP_RT_RECT;
				l_pstIrisRepresentation[index].ImageWidth = Convert.ToUInt16(rightWidth);
				l_pstIrisRepresentation[index].ImageHeight = Convert.ToUInt16(rightHeight);
				l_pstIrisRepresentation[index].BitDepth = 8;
				l_pstIrisRepresentation[index].ImageRange = 2;
				l_pstIrisRepresentation[index].EyeRollAngle = 0;
				l_pstIrisRepresentation[index].EyeRollAngleUncertainity = 0;
				l_pstIrisRepresentation[index].IrisCenterSmallX = 1;
				l_pstIrisRepresentation[index].IrisCenterLargeX = 2;
				l_pstIrisRepresentation[index].IrisCenterSmallY = 3;
				l_pstIrisRepresentation[index].IrisCenterLargeY = 4;
				l_pstIrisRepresentation[index].IrisDiameterSmall = 100;
				l_pstIrisRepresentation[index].IrisDiameterLarge = 200;
				l_pstIrisRepresentation[index].ImageLength = Convert.ToUInt32(rightWidth * rightHeight);
				l_pstIrisRepresentation[index].Image = pinnedArrayRight.AddrOfPinnedObject();
				++index;
			}

			if (picIso2011Left.Image != null)
			{
				l_pstIrisRepresentation[index] = new ISO2011_IRIS_REPRESENTATION();
				l_pstIrisRepresentation[index].CaptureDeviceTechID = 0;
				l_pstIrisRepresentation[index].CaptureDeviceVendorID = 0;
				l_pstIrisRepresentation[index].CaptureDeviceTypeID = Convert.ToUInt16(cboIsoDeviceId.SelectedIndex + 1);
				l_pstIrisRepresentation[index].NumberOfQualityBlocks = 0;
				//l_pstIrisRepresentation.QualityRecords;
				l_pstIrisRepresentation[index].EyeLabel = 2;
				l_pstIrisRepresentation[index].ImageType = 1;
				l_pstIrisRepresentation[index].ImageFormat = Constants.IMAGEFORMAT_MONO_RAW;
				l_pstIrisRepresentation[index].ImageProperties = Constants.IMG_PROP_RT_RECT;
				l_pstIrisRepresentation[index].ImageWidth = Convert.ToUInt16(leftWidth);
				l_pstIrisRepresentation[index].ImageHeight = Convert.ToUInt16(leftHeight);
				l_pstIrisRepresentation[index].BitDepth = 8;
				l_pstIrisRepresentation[index].ImageRange = 2;
				l_pstIrisRepresentation[index].EyeRollAngle = 0;
				l_pstIrisRepresentation[index].EyeRollAngleUncertainity = 0;
				l_pstIrisRepresentation[index].IrisCenterSmallX = 11;
				l_pstIrisRepresentation[index].IrisCenterLargeX = 21;
				l_pstIrisRepresentation[index].IrisCenterSmallY = 31;
				l_pstIrisRepresentation[index].IrisCenterLargeY = 41;
				l_pstIrisRepresentation[index].IrisDiameterSmall = 101;
				l_pstIrisRepresentation[index].IrisDiameterLarge = 201;
				l_pstIrisRepresentation[index].ImageLength = Convert.ToUInt32(leftWidth * leftHeight);
				l_pstIrisRepresentation[index].Image = pinnedArrayLeft.AddrOfPinnedObject();
			}

			object isoRecord;
			int isoRecordSize;
			if (0 !=
				(iResult =
				 _iDataIris.CreateISORecord2011(byNumberOfEyeRepresented, usIrisRepresentationCount, 
											byCertificationScheme, l_pstIrisRepresentation, out isoRecordSize, out isoRecord)))
			{
				if (iResult == Constants.IAIRIS_LICENCE_EXPIRED)
					MessageBox.Show(@"License expired!", Constants.Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
				else
					MessageBox.Show(string.Format(
					@"CreateISORecord failed with error  0x{0:X}", iResult),
					Constants.Title,
					MessageBoxButtons.OK,
					MessageBoxIcon.Information);
				return;
			}
			pinnedArrayRight.Free();
			pinnedArrayLeft.Free();

			string fileName = "ISO_" + Guid.NewGuid() + @".iso2011rec";
			if (!ByteArrayToFile(_isoDirectory, fileName, (byte[])isoRecord))
			{
				MessageBox.Show(
					@"Failed to save ISO record  " + Environment.NewLine + _isoDirectory,
					Constants.Title,
					MessageBoxButtons.OK,
					MessageBoxIcon.Information);
				return;
			}

			MessageBox.Show(string.Format(@"ISO2011 iris record saved successfully as {0}{1}\{2}", Environment.NewLine, _isoDirectory, fileName),
				Constants.Title,
				MessageBoxButtons.OK,
				MessageBoxIcon.Information);

		}

		private void btnReadISO2011_Click(object sender, EventArgs e)
		{
			int iResult = 0;

			OpenFileDialog openFileDialog1 = new OpenFileDialog
			{
				Filter = @"ISOIris Record(*.iso2011rec)|*.iso2011rec",
				InitialDirectory = _isoDirectory
			};

			if (openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				btnISO2011Clear_Click(sender, e);
				byte[] isoIrisRecord = File.ReadAllBytes(openFileDialog1.FileName);

				uint VersionNumber;
				if (0 !=
					(iResult =
					 _iDataIris.GetISORecordVersionNumber(isoIrisRecord, isoIrisRecord.Length, out VersionNumber)))
				{
					if (iResult == Constants.IAIRIS_LICENCE_EXPIRED)
						MessageBox.Show(@"License expired!", Constants.Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
					else
						MessageBox.Show(string.Format(
						@"GetISORecordVersionNumber failed with error  0x{0:X}", iResult),
						Constants.Title,
						MessageBoxButtons.OK,
						MessageBoxIcon.Information);
					return;
				}

				StringBuilder sb = new StringBuilder();
				sb.AppendLine("ISO record version: " + VersionNumber);

				ushort IrisRepresentationCount;
				byte CertificationScheme;
				byte NumberOfEyeRepresented;
				if (0 !=
					(iResult =
					 _iDataIris.GetISORecordInfo2011(isoIrisRecord, isoIrisRecord.Length, out IrisRepresentationCount,
												 out CertificationScheme, out NumberOfEyeRepresented)))
				{
					if (iResult == Constants.IAIRIS_LICENCE_EXPIRED)
						MessageBox.Show(@"License expired!", Constants.Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
					else
						MessageBox.Show(string.Format(
						@"GetISORecordInfo failed with error  0x{0:X}", iResult),
						Constants.Title,
						MessageBoxButtons.OK,
						MessageBoxIcon.Information);
					return;
				}

				sb.AppendLine("Representation count: " + IrisRepresentationCount);
				sb.AppendLine("Eyes represented: " + NumberOfEyeRepresented);
				sb.AppendLine(" ");

				//Try to display the images
				if (IrisRepresentationCount > 0)
				{
					ushort IrisRepresentationNumber = 1;
					uint IsDataValidationRequired = 0;
					ISO2011_IRIS_REPRESENTATION irisRepresentation;

					iResult = _iDataIris.GetImageFromISORecord2011(isoIrisRecord, isoIrisRecord.Length, IrisRepresentationNumber, IsDataValidationRequired, out irisRepresentation);
					if (iResult != 0)
					{
						if (iResult == Constants.IAIRIS_LICENCE_EXPIRED)
							MessageBox.Show(@"License expired!", Constants.Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
						else
							MessageBox.Show(string.Format(
							@"GetImageFromISORecord2011 failed for right eye with error  0x{0:X}", iResult),
							Constants.Title,
							MessageBoxButtons.OK,
							MessageBoxIcon.Information);
						return;
					}
					else
					{
						sb.AppendLine("DATA FOR FIRST IMAGE: ");
						sb.AppendLine("Capture device techID: " + irisRepresentation.CaptureDeviceTechID);
						sb.AppendLine("Capture device vendor ID: " + irisRepresentation.CaptureDeviceVendorID);
						sb.AppendLine("Capture device type ID: " + irisRepresentation.CaptureDeviceTypeID);
						sb.AppendLine("Quality block count: " + irisRepresentation.NumberOfQualityBlocks);
						sb.AppendLine("Eye label: " + irisRepresentation.EyeLabel);
						sb.AppendLine("Image type: " + irisRepresentation.ImageType);
						sb.AppendLine("Image format: " + irisRepresentation.ImageFormat);
						sb.AppendLine("Image properties: " + irisRepresentation.ImageProperties);
						sb.AppendLine("Image width: " + irisRepresentation.ImageWidth);
						sb.AppendLine("Image height: " + irisRepresentation.ImageHeight);
						sb.AppendLine("Bit depth: " + irisRepresentation.BitDepth);
						sb.AppendLine("Image range: " + irisRepresentation.ImageRange);
						sb.AppendLine("Eye roll angle: " + irisRepresentation.EyeRollAngle);
						sb.AppendLine("Eye roll angle uncertainity: " + irisRepresentation.EyeRollAngleUncertainity);
						sb.AppendLine("Eye center small x: " + irisRepresentation.IrisCenterSmallX);
						sb.AppendLine("Eye center large x: " + irisRepresentation.IrisCenterLargeX);
						sb.AppendLine("Eye center small y: " + irisRepresentation.IrisCenterSmallY);
						sb.AppendLine("Eye center large y: " + irisRepresentation.IrisCenterLargeY);
						sb.AppendLine("Diameter small: " + irisRepresentation.IrisDiameterSmall);
						sb.AppendLine("Diameter large: " + irisRepresentation.IrisDiameterLarge);

						byte[] ISOImage = new byte[irisRepresentation.ImageLength];
						Marshal.Copy((IntPtr)irisRepresentation.Image, ISOImage, 0, Convert.ToInt32(irisRepresentation.ImageLength));

						byte[] RawImage = null;
						int rawWidth = 0, rawHeight = 0;
						if (irisRepresentation.ImageFormat == Constants.ISO2011_IMAGEFORMAT_MONO_JPEG2000)
							ConvertImageToRaw(enmImageFormat.JPEG2K, ISOImage, out RawImage, out rawWidth, out rawHeight);
						else if (irisRepresentation.ImageFormat == Constants.ISO2011_IMAGEFORMAT_MONO_PNG)
							ConvertImageToRaw(enmImageFormat.PNG, ISOImage, out RawImage, out rawWidth, out rawHeight);
						else
						{
							rawWidth = irisRepresentation.ImageWidth;
							rawHeight = irisRepresentation.ImageHeight;
							RawImage = new byte[rawWidth * rawHeight];
							ISOImage.CopyTo(RawImage, 0);
						}
						if (irisRepresentation.EyeLabel == 0 || irisRepresentation.EyeLabel == 1)
							picIso2011Right.Image = ImageHelper.Raw8BitByteArrayToImage(RawImage, rawWidth, rawHeight);
						else
							picIso2011Left.Image = ImageHelper.Raw8BitByteArrayToImage(RawImage, rawWidth, rawHeight);
					}
				}
				lblIso2011RecordInfo.Text = sb.ToString();
				btnCreateISO2011.Enabled = false;
			}
		}

		private void btnISO2011Clear_Click(object sender, EventArgs e)
		{
			picIso2011Right.Image = null;
			picIso2011Left.Image = null;
			lblIso2011RecordInfo.Text = string.Empty;
			btnCreateISO2011.Enabled = false;
		}

        private void btnVerify_Click(object sender, EventArgs e)
        {
            int iResult;
            int iGalleryImageWidth;
            int iGalleryImageHeight;

            lblMatchResults.Text = string.Empty;

            byte[] byGalleryImageRaw = ImageHelper.ImageToRaw8BitByteArray(picGallery.Image, out iGalleryImageWidth,
                                                                         out iGalleryImageHeight);
            int iProbeImageHeight;
            int iProbeImageWidth;
            byte[] byProbeImageRaw = ImageHelper.ImageToRaw8BitByteArray(picProbe.Image, out iProbeImageWidth,
                                                                       out iProbeImageHeight);
            object oProbeIrisCode;
            ImageInfo stProbeImageInfo;
            iResult = _iDataIris.CreateLongIrisCode(Constants.IRIS_IMAGE_RECT, byProbeImageRaw, iProbeImageWidth,
                                               iProbeImageHeight, out oProbeIrisCode, out stProbeImageInfo);
            if (iResult != 0)
            {
                if (iResult == Constants.IAIRIS_LICENCE_EXPIRED)
                    MessageBox.Show(@"License expired!", Constants.Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show(string.Format(
                    @"CreateLongIrisCode failed with error  0x{0:X}", iResult),
                    Constants.Title,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            object oGalleryIrisCode;
            ImageInfo stGalleryImageInfo;
            object oGalleryProcessedImage;
            int iGalleryImageQuality = 0;
            iResult = _iDataIris.CreateIrisCode(Constants.IRIS_IMAGE_RECT, byGalleryImageRaw, iGalleryImageWidth,
                                               iGalleryImageHeight, out oGalleryIrisCode, out oGalleryProcessedImage,
                                               out iGalleryImageQuality, out stGalleryImageInfo);
            if (iResult != 0)
            {
                if (iResult == Constants.IAIRIS_LICENCE_EXPIRED)
                    MessageBox.Show(@"License expired!", Constants.Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show(string.Format(
                    @"CreateIrisCode failed with error  0x{0:X}", iResult),
                    Constants.Title,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            float hd;
			iResult = _iDataIris.VerifyByLongIrisCode(Constants.MATCHING_MODE_STANDARD, oProbeIrisCode, oGalleryIrisCode,
                                                0.32f, out hd);
            if (iResult != 0 && iResult != Constants.IAIRIS_NOT_MATCHED)
            {
                if (iResult == Constants.IAIRIS_LICENCE_EXPIRED)
                    MessageBox.Show(@"License expired!", Constants.Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show(string.Format(
                    @"VerifyByLongIrisCode failed with error  0x{0:X}", iResult),
                    Constants.Title,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            if (iResult == Constants.IAIRIS_NOT_MATCHED)
            {
				lblMatchResults.Text += @"VERIFY:  FAILED";
            }
            else
            {
                lblMatchResults.Text += @"Hamming Distance:  " + hd.ToString("0.00") + Environment.NewLine;
                lblMatchResults.Text += @"VERIFY:  SUCCESSFUL";
            }
        }

        private void btnMatchImage_Click(object sender, EventArgs e)
        {
			int iResult = 0;
            int iGalleryImageWidth;
            int iGalleryImageHeight;

            lblMatchResults.Text = string.Empty;

			byte[] byGalleryImageRaw = ImageHelper.ImageToRaw8BitByteArray(picGallery.Image, out iGalleryImageWidth,
                                                                         out iGalleryImageHeight);
            int iProbeImageHeight;
            int iProbeImageWidth;
            byte[] byProbeImageRaw = ImageHelper.ImageToRaw8BitByteArray(picProbe.Image, out iProbeImageWidth,
                                                                       out iProbeImageHeight);
            

            object oGalleryIrisCode;
            ImageInfo galleryImageInfo;
            object oGalleryProcessedImage;
            int iGalleryImageQuality = 0;
            iResult = _iDataIris.CreateIrisCode(Constants.IRIS_IMAGE_RECT, byGalleryImageRaw, iGalleryImageWidth,
                                               iGalleryImageHeight, out oGalleryIrisCode, out oGalleryProcessedImage,
                                               out iGalleryImageQuality, out galleryImageInfo);
			if (iResult != 0)
			{
				if (iResult == Constants.IAIRIS_LICENCE_EXPIRED)
					MessageBox.Show(@"License expired!", Constants.Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
				else
					MessageBox.Show(string.Format(
					@"CreateIrisCode failed with error  0x{0:X}", iResult),
					Constants.Title,
					MessageBoxButtons.OK,
					MessageBoxIcon.Information);
				return;
			}

            int matchedIndex = 0;

            float hd = 0.0f;
            iResult = _iDataIris.MatchByIrisImage(Constants.MATCHING_MODE_STANDARD, byProbeImageRaw, 100, iProbeImageWidth, iProbeImageHeight, oGalleryIrisCode,
                                                1, 0.32f, ref matchedIndex, out hd);
			if (iResult != 0 && iResult != Constants.IAIRIS_NOT_MATCHED)
			{
				if (iResult == Constants.IAIRIS_LICENCE_EXPIRED)
					MessageBox.Show(@"License expired!", Constants.Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
				else
					MessageBox.Show(string.Format(
                    @"MatchByIrisImage failed with error  0x{0:X}", iResult),
					Constants.Title,
					MessageBoxButtons.OK,
					MessageBoxIcon.Information);
				return;
			}

            lblMatchResults.Text += @"Matched Index:  " + matchedIndex + Environment.NewLine;
            lblMatchResults.Text += @"Hamming Distance:  " + hd.ToString("0.00") + Environment.NewLine;

            if (matchedIndex < 0)
            {
                lblMatchResults.Text += @"MATCH:  FAILED";
            }
            else
            {
                lblMatchResults.Text += @"MATCH:  SUCCESSFUL";
            }
        }

        

        private void btnVerifyImage_Click(object sender, EventArgs e)
        {

            int iResult = 0;
            int galleryImageWidth;
            int galleryImageHeight;

            lblMatchResults.Text = string.Empty;

            byte[] rawGalleryImage = ImageHelper.ImageToRaw8BitByteArray(picProbe.Image, out galleryImageWidth,
                                                                         out galleryImageHeight);
            int probeImageHeight;
            int probeImageWidth;
            byte[] rawProbeImage = ImageHelper.ImageToRaw8BitByteArray(picGallery.Image, out probeImageWidth,
                                                                       out probeImageHeight);


            object probeIrisCode;
            ImageInfo probeImageInfo;
            object probeProcessedImage;
            int probeImageQuality = 0;
            iResult = _iDataIris.CreateIrisCode(Constants.IRIS_IMAGE_RECT, rawProbeImage, probeImageWidth,
                                               probeImageHeight, out probeIrisCode, out probeProcessedImage,
                                               out probeImageQuality, out probeImageInfo);
            if (iResult != 0)
            {
                if (iResult == Constants.IAIRIS_LICENCE_EXPIRED)
                    MessageBox.Show(@"License expired!", Constants.Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show(string.Format(
                    @"CreateIrisCode failed with error  0x{0:X}", iResult),
                    Constants.Title,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            float hd;
            iResult = _iDataIris.VerifyByIrisImage(Constants.MATCHING_MODE_STANDARD,100, rawGalleryImage,galleryImageWidth, galleryImageHeight, probeIrisCode,
                                                 0.32f, out hd);
            if (iResult != 0 && iResult != Constants.IAIRIS_NOT_MATCHED)
            {
                if (iResult == Constants.IAIRIS_LICENCE_EXPIRED)
                    MessageBox.Show(@"License expired!", Constants.Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show(string.Format(
                    @"VerifyIrisImage failed with error  0x{0:X}", iResult),
                    Constants.Title,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

        

            if (iResult == Constants.IAIRIS_NOT_MATCHED)
            {
				lblMatchResults.Text += @"VERIFY:  FAILED";
            }
            else
            {
                lblMatchResults.Text += @"Hamming Distance:  " + hd.ToString("0.00") + Environment.NewLine;
                lblMatchResults.Text += @"VERIFY:  SUCCESSFUL";
            }
        }

        private void btnLoadRAWCompress_Click(object sender, EventArgs e)
        {
            byte[] rawImageBytes;
            int iResult = 0;
            int width, height;
            int rawWidth = 0;
            int rawHeight = 0;
            object Jp2ImageBytes;
            int iJp2ImgSize;
            object tmpImage;
			btnLoadClear_Click(sender, null);

            if (LoadIrisImage(out rawImageBytes, out width, out height) != 0)
                return;

            if (null == rawImageBytes) return;

            //Compress rawImageBytes to JPG2K 10KB. iDataIrisSDK NEW API.
            iResult = _iDataIris.RawToJpeg2kToSize(rawImageBytes, width, height,1024 * 3,
                                    out  Jp2ImageBytes, out iJp2ImgSize
                                    );
            if (iResult != 0)
            {
                if (iResult == Constants.IAIRIS_LICENCE_EXPIRED)
                    MessageBox.Show(@"License expired!", Constants.Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show(string.Format(
                    @" RawToJpeg2kToSize failed with error  0x{0:X}", iResult),
                    Constants.Title,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

			lblImageQuality.Text = string.Empty;
			lblImageQuality.Text += @"RawToJpeg2kToSize: SUCCESSFULL" + Environment.NewLine;
 
            //Convert JPG2KCompressedImageBytes to RAW 

            iResult = _iDataIris.Jpeg2kToRaw(Jp2ImageBytes, iJp2ImgSize, out tmpImage, out width, out height);
            if (iResult == 0)
            {
                rawImageBytes = tmpImage as byte[];
                rawWidth = width;
                rawHeight = height;
            }
            else
            {
                if (iResult == Constants.IAIRIS_LICENCE_EXPIRED)
                    MessageBox.Show(@"License expired!", Constants.Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show(string.Format(
                    @"Jpeg2kToRaw failed for left eye with error  0x{0:X}", iResult),
                    Constants.Title,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return ;
            }
			lblImageQuality.Text += @"Jpeg2kToRaw: SUCCESSFULL" + Environment.NewLine;

            picOriginal.Image = ImageHelper.Raw8BitByteArrayToImage(rawImageBytes, width, height);

            int quality;

            IrisImageQualityInfo irisImageQualityInfo;
            iResult = _iDataIris.GetImageQuality(Constants.IRIS_IMAGE_RECT, rawImageBytes, width, height, out quality,
                                       out irisImageQualityInfo);
            if (iResult != 0)
            {
                if (iResult == Constants.IAIRIS_LICENCE_EXPIRED)
                    MessageBox.Show(@"License expired!", Constants.Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show(string.Format(
                    @"GetImageQuality failed with error  0x{0:X}", iResult),
                    Constants.Title,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                //return;
            }
            lblImageQuality.Text += @"Quality Score: " + quality + Environment.NewLine;
            lblImageQuality.Text += @"IrisRadius: " + irisImageQualityInfo.IrisRadius.value + Environment.NewLine;
            lblImageQuality.Text += @"IrisFocus: " + irisImageQualityInfo.IrisFocus.value + Environment.NewLine;
            lblImageQuality.Text += @"IrisVisibility: " + irisImageQualityInfo.IrisVisibility.value +
                                    Environment.NewLine;
            lblImageQuality.Text += @"SNR: " + irisImageQualityInfo.SNR.value + Environment.NewLine;
            lblImageQuality.Text += @"IrisPupilContrast: " + irisImageQualityInfo.IrisPupilContrast.value +
                                    Environment.NewLine;
            lblImageQuality.Text += @"ScleraIrisContrast: " + irisImageQualityInfo.ScleraIrisContrast.value +
                                    Environment.NewLine;
            
        }

		private void btnPIVClear_Click(object sender, EventArgs e)
		{
			PIVResetControls();
		}

		byte[] g_rawImageBytes;
		int g_rawImagewidth, g_rawImageheight;
		int g_cropImageWidth = 0;
		int g_cropImageHeight = 0;
		object g_croppedImage = null;
		byte[] g_jp2kCompressedBytes;
		byte[] g_CompressedRAWImageBytes;
		int g_CompressedRAWImageWidth, g_CompressedRAWImageHeight;
		SaveFileDialog saveFileDialog = new SaveFileDialog();
        byte[] g_ProcessedImageBytes;
        int g_ProcessedImageWidth;
        int ProcessedImageHeight;

		private void PIVResetControls()
		{
			picPIVImage.Image = null;
			picPIVCropMask.Image = null;
			picPIVCompress.Image = null;
			lblImageQuality.Text = string.Empty;
			lblPIVLoadImage.Text = "Load Image";
			btnPIVLoadImage.Visible = true;
			btnPIVCropMask.Visible = true;
			btnPIVCompress.Visible = true;
			btnPIVLoadImage.Enabled = true;
			btnPIVCropMask.Enabled = false;
			btnPIVCompress.Enabled = false;
			btnPIVSave.Enabled = false;
			btnPIVMatch.Enabled = false;
			txtPIVMessage.Text = string.Empty;
		}

		private void FrmMain_Load(object sender, EventArgs e)
		{
			PIVResetControls();
            ProcessingImageReset();
		}

		private int LoadPIVImage(out byte[] rawImageBytes, out int rawWidth, out int rawHeight, out string strFile, out string strExtension)
		{
			rawImageBytes = null;
			rawWidth = 640;
			rawHeight = 480;
			strFile = string.Empty;
			strExtension = string.Empty;
			string strMessage = string.Empty;

			OpenFileDialog openFileDialog1 = new OpenFileDialog();

			openFileDialog1.Filter =
				@"Jpeg/Jpg files (*.jpeg/jpg)|*.jpeg;*.jpg|Jpeg2000 files (*.jp2)|*.jp2|Png (*.png)|*.png";
			openFileDialog1.RestoreDirectory = true;

			int iResult = 0;
			if (openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				byte[] loadedImageBuffer = File.ReadAllBytes(openFileDialog1.FileName);
				strFile = openFileDialog1.FileName;
				object tmpImage;
				int width, height;

				if (openFileDialog1.FilterIndex == 1) //JPEG
				{
					strExtension = "JPEG";
					if (0 ==
						(iResult =
						 _iDataIris.JpegToRaw(loadedImageBuffer, loadedImageBuffer.Length, out tmpImage, out width,
											  out height)))
					{
						rawImageBytes = tmpImage as byte[];
						rawWidth = width;
						rawHeight = height;
					}
					else
					{
						if (iResult == Constants.IAIRIS_LICENCE_EXPIRED)
							strMessage = string.Format("License expired!{0}", Environment.NewLine);
						else
							strMessage = string.Format(@"JpegToRaw failed for left eye with error  0x{0:X}{1}", iResult, Environment.NewLine);
						DisplayMessage(strMessage);
						return -1;
					}
				}

				if (openFileDialog1.FilterIndex == 2) //JPEG2000 File
				{
					strExtension = "JPEG2K";
					if (0 ==
						(iResult =
						 _iDataIris.Jpeg2kToRaw(loadedImageBuffer, loadedImageBuffer.Length, out tmpImage, out width,
												out height)))
					{
						rawImageBytes = tmpImage as byte[];
						rawWidth = width;
						rawHeight = height;
					}
					else
					{
						if (iResult == Constants.IAIRIS_LICENCE_EXPIRED)
							strMessage = string.Format("License expired!{0}", Environment.NewLine);
						else
							strMessage = string.Format(@"Jpeg2kToRaw failed for left eye with error  0x{0:X}{1}", iResult, Environment.NewLine);
						DisplayMessage(strMessage);
						return -1;
					}
				}

				if (openFileDialog1.FilterIndex == 3) //Png File
				{
					strExtension = "PNG";
					if (0 ==
						(iResult =
						 _iDataIris.PngToRaw(loadedImageBuffer, loadedImageBuffer.Length, out tmpImage, out width,
											 out height)))
					{
						rawImageBytes = tmpImage as byte[];
						rawWidth = width;
						rawHeight = height;
					}
					else
					{
						if (iResult == Constants.IAIRIS_LICENCE_EXPIRED)
							strMessage = string.Format("License expired!{0}", Environment.NewLine);
						else
							strMessage = string.Format(@"PngToRaw failed for left eye with error  0x{0:X}{1}", iResult, Environment.NewLine);
						DisplayMessage(strMessage);
						return -1;
					}
				}
			}
			else
				return -1;
			return 0;
		}

		private void btnPIVLoadImage_Click(object sender, EventArgs e)
		{
			string strMessage = string.Empty;
			string strFile = string.Empty;
			string strExtension = string.Empty;
			if (LoadPIVImage(out g_rawImageBytes, out g_rawImagewidth, out g_rawImageheight, out strFile, out strExtension) != 0)
				return;

			if (null == g_rawImageBytes)
			{
				strMessage = string.Format(@"Failed to load the file {0}{1}", strFile, Environment.NewLine);
				DisplayMessage(strMessage);
				return;
			}

			picPIVImage.Image = ImageHelper.Raw8BitByteArrayToImage(g_rawImageBytes, g_rawImagewidth, g_rawImageheight);
			strMessage = string.Format(@"Loaded the image from file {0}{1}", strFile, Environment.NewLine);
			strMessage += string.Format("Raw image size: {0} bytes{1}", g_rawImagewidth * g_rawImageheight, Environment.NewLine);
			DisplayMessage(strMessage);
			lblPIVLoadImage.Text = String.Format("{0} Image", strExtension);
			btnPIVCropMask.Enabled = true;
			btnPIVLoadImage.Enabled = false;
			btnPIVLoadImage.Visible = false;
		}

		private void btnPIVCropMask_Click(object sender, EventArgs e)
		{
			int cropTopX = 0;
			int cropTopY = 0;
			string strMessage = string.Empty;

			int iResult = _iDataIris.CropIrisImage(Constants.CROP_TYPE_CROP_AND_MASKED, g_rawImageBytes, g_rawImagewidth, g_rawImageheight, out cropTopX,
									 out cropTopY, out g_cropImageWidth,
									 out g_cropImageHeight, out g_croppedImage);
			if (iResult != 0)
			{
				if (iResult == Constants.IAIRIS_LICENCE_EXPIRED)
					strMessage = string.Format("License expired!{0}", Environment.NewLine);
				else
					strMessage = string.Format(@"CropIrisImage failed with error  0x{0:X}{1}", iResult, Environment.NewLine);
				DisplayMessage(strMessage);
				return;
			}
			strMessage = string.Format(@"CropIrisImage SUCCESSFUL{0}", Environment.NewLine);
			strMessage += string.Format("Cropped image size: {0} bytes{1}", g_cropImageWidth * g_cropImageHeight, Environment.NewLine);
			DisplayMessage(strMessage);

			int cropImageWidthAdj = ((g_cropImageWidth + 3) / 4) * 4;
			int cropImageHeightAdj = ((g_cropImageHeight + 3) / 4) * 4;
			byte[] croppedImageAdj = new byte[cropImageWidthAdj * cropImageHeightAdj];

			for (int i = 0; i < g_cropImageHeight; i++)
			{
				Array.Copy((byte[])g_croppedImage, i * g_cropImageWidth, croppedImageAdj, i * cropImageWidthAdj,
						   g_cropImageWidth);
			}
			picPIVCropMask.Image = ImageHelper.Raw8BitByteArrayToImage((croppedImageAdj), cropImageWidthAdj,
																   cropImageHeightAdj);
			btnPIVCompress.Enabled = true;
			btnPIVCropMask.Enabled = false;
			btnPIVCropMask.Visible = false;		
		}

		private void DisplayMessage(string strMessage)
		{
			txtPIVMessage.Text += strMessage;
			txtPIVMessage.Select(txtPIVMessage.TextLength, 0);
			if (txtPIVMessage.ScrollBars.ToString() == ScrollBars.Vertical.ToString())
			{
				txtPIVMessage.ScrollToCaret();
			}

		}

		private void btnPIVCompress_Click(object sender, EventArgs e)
		{
			string strMessage = string.Empty;
			int iResult = 0;
			object Jp2ImageBytes;
			int iJp2ImgSize;
			object tmpImage;
			int iOutNumber;


			string inputText =  Microsoft.VisualBasic.Interaction.InputBox("Enter the required file size in Kb", "Compress Image", "3", -1, -1);
			if (!int.TryParse(inputText, out iOutNumber))
			{
				strMessage = string.Format(@"Input value not correct!{0}", Environment.NewLine);
				DisplayMessage(strMessage);
				return;
			}
			//Compress rawImageBytes to JPG2K 3KB. iDataIrisSDK NEW API.
			iResult = _iDataIris.RawToJpeg2kToSize(g_croppedImage, g_cropImageWidth, g_cropImageHeight, 1024 * iOutNumber,
									out  Jp2ImageBytes, out iJp2ImgSize
									);
			if (iResult != 0)
			{
				if (iResult == Constants.IAIRIS_LICENCE_EXPIRED)
					strMessage = string.Format(@"License expired!{0}", Environment.NewLine);
				else
				{
					strMessage = string.Format(@"RawToJpeg2kToSize failed with error  0x{0:X}{1}", iResult, Environment.NewLine);
					if (-2063597520 == iResult)
						strMessage += string.Format(@"	Input compression size too small{0}", Environment.NewLine);
				}
				DisplayMessage(strMessage);
				return;
			}

			strMessage = "RawToJpeg2kToSize: SUCCESSFULL" + Environment.NewLine;
			strMessage += string.Format("Compressed image size: {0} bytes{1}", iJp2ImgSize, Environment.NewLine);
			DisplayMessage(strMessage);

			//Convert JPG2KCompressedImageBytes to RAW 
           
			iResult = _iDataIris.Jpeg2kToRaw(Jp2ImageBytes, iJp2ImgSize, out tmpImage, out g_CompressedRAWImageWidth, out g_CompressedRAWImageHeight);
			if (iResult == 0)
			{
				g_jp2kCompressedBytes = Jp2ImageBytes as byte[];
				g_CompressedRAWImageBytes = tmpImage as byte[];
			}
			else
			{
				if (iResult == Constants.IAIRIS_LICENCE_EXPIRED)
					strMessage = string.Format(@"License expired!{0}", Environment.NewLine);
				else
					strMessage = string.Format(@"Jpeg2kToRaw failed for left eye with error  0x{0:X}{1}", iResult, Environment.NewLine);
				DisplayMessage(strMessage);
				return;
			}
			strMessage = "Jpeg2kToRaw: SUCCESSFULL" + Environment.NewLine;
			DisplayMessage(strMessage);

			picPIVCompress.Image = ImageHelper.Raw8BitByteArrayToImage(tmpImage as byte[], g_CompressedRAWImageWidth, g_CompressedRAWImageHeight);
			btnPIVSave.Enabled = true;
			btnPIVMatch.Enabled = true;
			btnPIVCompress.Enabled = false;
			btnPIVCompress.Visible = false;
		}

		private void btnPIVSave_Click(object sender, EventArgs e)
		{
			string strMessage = string.Empty;
			string fileName = string.Empty;
			saveFileDialog.Filter = @"Jpeg2000 files (*.jp2)|*.jp2";
			saveFileDialog.RestoreDirectory = true;
			saveFileDialog.InitialDirectory = _isoDirectory;

			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				fileName = saveFileDialog.FileName;

				if (!ByteArrayToFile(Path.GetDirectoryName(fileName), Path.GetFileName(fileName), g_jp2kCompressedBytes))
				{
					strMessage = string.Format(@"Failed to save Image  {0}{1}\{2}{3}", Environment.NewLine, _isoDirectory, fileName, Environment.NewLine);
					DisplayMessage(strMessage);
					return;
				}

				strMessage = String.Format(@"Image saved successfully as {0}{1}\{2}{3}", Environment.NewLine, _isoDirectory, fileName, Environment.NewLine);
				DisplayMessage(strMessage);
			}
		}

		private void btnPIVMatch_Click(object sender, EventArgs e)
		{
			int iResult;
			string strMessage = string.Empty;

			MatchByIrisCode(g_rawImageBytes, g_rawImagewidth, g_rawImageheight, g_CompressedRAWImageBytes, g_CompressedRAWImageWidth, g_CompressedRAWImageHeight, out strMessage, out iResult);
			DisplayMessage(strMessage);
		}

        private void btnLoadImageTemplateCreation_Click(object sender, EventArgs e)
        {
            int iResult;
            lblTemplateImageResult.Text = string.Empty;
            if (LoadIrisImage(out g_ProcessedImageBytes, out g_ProcessedImageWidth, out ProcessedImageHeight) != 0)
                return;

            if (null == g_ProcessedImageBytes) 
                return;

            picIrisTemplate.Image = ImageHelper.Raw8BitByteArrayToImage(g_ProcessedImageBytes, g_ProcessedImageWidth, ProcessedImageHeight);

            object oTemplateIrisCode = null;
            ImageInfo stGalleryImageInfo;
            object oTemplateProcessedImage = null;
            int iTemplateImageQuality = 0;

            iResult = _iDataIris.CreateIrisCode(Constants.IRIS_IMAGE_RECT, g_ProcessedImageBytes, g_ProcessedImageWidth,
                                               ProcessedImageHeight, out oTemplateIrisCode, out oTemplateProcessedImage,
                                               out iTemplateImageQuality, out stGalleryImageInfo);
            if (iResult != 0)
            {
                if (iResult == Constants.IAIRIS_LICENCE_EXPIRED)
                    MessageBox.Show(@"License expired!", Constants.Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show(string.Format(
                    @"CreateIrisCode failed with error  0x{0:X}", iResult),
                    Constants.Title,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            int cropTopX = 0;
            int cropTopY = 0;
            int cropImageWidth = 0;
            int cropImageHeight = 0;
            object croppedImage = null;

            iResult = _iDataIris.CropIrisImage(Constants.CROP_TYPE_CROPPED, oTemplateProcessedImage, g_ProcessedImageWidth, ProcessedImageHeight, out cropTopX,
                                     out cropTopY, out cropImageWidth,
                                     out cropImageHeight, out croppedImage);
            if (iResult != 0)
            {
                if (iResult == Constants.IAIRIS_LICENCE_EXPIRED)
                    MessageBox.Show(@"License expired!", Constants.Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show(string.Format(
                    @"CropIrisImage failed with error  0x{0:X}", iResult),
                    Constants.Title,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            g_ProcessedImageWidth = ((cropImageWidth + 3) / 4) * 4;
            ProcessedImageHeight = ((cropImageHeight + 3) / 4) * 4;
            g_ProcessedImageBytes = new byte[g_ProcessedImageWidth * ProcessedImageHeight];

            for (int i = 0; i < cropImageHeight; i++)
            {
                Array.Copy((byte[])croppedImage, i * cropImageWidth, g_ProcessedImageBytes, i * g_ProcessedImageWidth,
                           cropImageWidth);
            }

            picProcessedImage.Image = ImageHelper.Raw8BitByteArrayToImage((byte[])g_ProcessedImageBytes, g_ProcessedImageWidth,
                                                                   ProcessedImageHeight);
            btnSaveProcessedImage.Enabled = true;
            g_ProcessedImageBytes = croppedImage as byte[];
            lblTemplateImageResult.Text += @"Image Quality:  " + iTemplateImageQuality + Environment.NewLine;
            lblTemplateImageResult.Text += @"IrisCode Response:  SUCCESSFULL" + Environment.NewLine;
        }

        private void btnClearTemplateCreation_Click(object sender, EventArgs e)
        {
            ProcessingImageReset();
        }
        
        private void ProcessingImageReset()
        {
            picIrisTemplate.Image = null;
            picProcessedImage.Image = null;
            lblTemplateImageResult.Text = string.Empty; 
            
            btnLoadImageTemplateCreation.Visible = true;
            btnSaveProcessedImage.Enabled = false;
        }
        private void btnSaveProcessedImage_Click(object sender, EventArgs e)
        {
            string _FileName = string.Format("Image_{0}_{1}.raw", g_ProcessedImageWidth, ProcessedImageHeight);
            try
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                // Open file for reading
                System.IO.FileStream _FileStream = new System.IO.FileStream(_FileName, System.IO.FileMode.Create,
                                            System.IO.FileAccess.Write);
                // Writes a block of bytes to this stream using data from
                // a byte array.
                _FileStream.Write(g_ProcessedImageBytes, 0, g_ProcessedImageBytes.Length);

                // close file stream
                _FileStream.Close();
                MessageBox.Show(string.Format(@"{0} saved successfully ", _FileName),
                Constants.Title,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            }
            catch (Exception _Exception)
            {
                // Error
                Console.WriteLine("Exception caught in process: {0}",
                                  _Exception.ToString());
            }
        }
    }
}
