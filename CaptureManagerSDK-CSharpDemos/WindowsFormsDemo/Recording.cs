using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;

using CaptureManagerToCSharpProxy;
using CaptureManagerToCSharpProxy.Interfaces;

using WPFRecording;

namespace WindowsFormsDemo
{
    public partial class Recording : Form
    {
        CaptureManager mCaptureManager = null;

        ISession mSession = null;

        ISessionControl mISessionControl = null;

        ISinkControl mSinkControl = null;

        ISourceControl mSourceControl = null;

        IEncoderControl mEncoderControl = null;

        AbstractSink mSink = null;

        public XmlNode mSelectedSourceXmlNode = null;

        class ContainerItem
        {
            public string mFriendlyName = "SourceItem";

            public XmlNode mXmlNode;

            public override string ToString()
            {
                return mFriendlyName;
            }
        }

        public Recording()
        {
            InitializeComponent();

            try
            {
                mCaptureManager = new CaptureManager("CaptureManager.dll");
            }
            catch (System.Exception exc)
            {
                try
                {
                    mCaptureManager = new CaptureManager();
                }
                catch (System.Exception exc1)
                {

                }
            }

            if (mCaptureManager == null)
            {
                return;
            }

            mSourceControl = mCaptureManager.createSourceControl();

            mISessionControl = mCaptureManager.createSessionControl();

            mSinkControl = mCaptureManager.createSinkControl();

            mEncoderControl = mCaptureManager.createEncoderControl();

            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();

            string lxmldoc = "";

            mCaptureManager.getCollectionOfSources(ref lxmldoc);

            if (string.IsNullOrEmpty(lxmldoc))
            {
                return;
            }

            doc.LoadXml(lxmldoc);

            XmlNodeList lSourceNodes = doc.DocumentElement.ChildNodes;// .SelectNodes("//*[Source.Attributes/Attribute[@Name='MF_DEVSOURCE_ATTRIBUTE_MEDIA_TYPE']/Value.ValueParts/ValuePart[@Value='MFMediaType_Video']]");

            if (lSourceNodes != null)
            {
                foreach (object item in lSourceNodes)
                {
                    XmlNode lNode = (XmlNode)item;

                    if (lNode != null)
                    {
                        XmlNode lvalueNode = lNode.SelectSingleNode("Source.Attributes/Attribute[@Name='MF_DEVSOURCE_ATTRIBUTE_FRIENDLY_NAME']/SingleValue/@Value");

                        ContainerItem lSourceItem = new ContainerItem()
                        {
                            mFriendlyName = lvalueNode.Value,
                            mXmlNode = lNode
                        };

                        sourceComboBox.Items.Add(lSourceItem);
                    }


                }
            }
        }

        private void sourceComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

            ContainerItem lSelectedSourceItem = (ContainerItem)sourceComboBox.SelectedItem;

            if (lSelectedSourceItem == null)
            {
                return;
            }

            XmlNodeList lSubTypesNode = lSelectedSourceItem.mXmlNode.SelectNodes("PresentationDescriptor/StreamDescriptor/MediaTypes/MediaType/MediaTypeItem[@Name='MF_MT_SUBTYPE']/SingleValue/@Value");

            if (lSubTypesNode == null)
            {
                return;
            }

            mSelectedSourceXmlNode = lSelectedSourceItem.mXmlNode;

            streamComboBox.Items.Clear();

            foreach (XmlNode item in lSubTypesNode)
            {
                string lSubType = item.Value.Replace("MFVideoFormat_", "");

                lSubType = lSubType.Replace("MFAudioFormat_", "");

                if (!streamComboBox.Items.Contains(lSubType))
                {
                    streamComboBox.Items.Add(lSubType);
                }
            }
        }

        private void streamComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string lCurrentSubType = (string)streamComboBox.SelectedItem;

            XmlNode lCurrentSourceNode = mSelectedSourceXmlNode as XmlNode;

            if (lCurrentSourceNode == null)
            {
                return;
            }

            XmlNodeList lMediaTypeNodes = lCurrentSourceNode.SelectNodes("PresentationDescriptor/StreamDescriptor/MediaTypes/MediaType[MediaTypeItem[@Name='MF_MT_SUBTYPE']/SingleValue[@Value='MFVideoFormat_" + lCurrentSubType + "']]");

            if (lMediaTypeNodes == null)
            {
                return;
            }

            if (lMediaTypeNodes.Count == 0)
            {

                lMediaTypeNodes = lCurrentSourceNode.SelectNodes("PresentationDescriptor/StreamDescriptor/MediaTypes/MediaType[MediaTypeItem[@Name='MF_MT_SUBTYPE']/SingleValue[@Value='MFAudioFormat_" + lCurrentSubType + "']]");

                if (lMediaTypeNodes == null)
                {
                    return;
                }

                if (lMediaTypeNodes.Count == 0)
                {
                    return;
                }
            }

            mediaTypeComboBox.Items.Clear();

            foreach (object item in lMediaTypeNodes)
            {
                XmlNode lNode = (XmlNode)item;

                if (lNode != null)
                {

                    XmlNode lStreamNode = lCurrentSourceNode.SelectSingleNode("PresentationDescriptor/StreamDescriptor");

                    if (lStreamNode == null)
                    {
                        return;
                    }

                    XmlNode lvalueNode = lStreamNode.SelectSingleNode("@MajorType");

                    string mTitle = "";

                    if (lvalueNode != null && lvalueNode.Value == "MFMediaType_Video")
                    {

                        lvalueNode = lNode.SelectSingleNode("MediaTypeItem[@Name='MF_MT_FRAME_SIZE']/Value.ValueParts/ValuePart[1]/@Value");

                        mTitle = lvalueNode.Value;

                        lvalueNode = lNode.SelectSingleNode("MediaTypeItem[@Name='MF_MT_FRAME_SIZE']/Value.ValueParts/ValuePart[2 ]/@Value");

                        mTitle += "x" + lvalueNode.Value;

                        lvalueNode = lNode.SelectSingleNode("MediaTypeItem[@Name='MF_MT_FRAME_RATE']/RatioValue/@Value");

                        mTitle += ", " + lvalueNode.Value + " FPS, ";

                        lvalueNode = lNode.SelectSingleNode("MediaTypeItem[@Name='MF_MT_SUBTYPE']/SingleValue/@Value");

                        mTitle += lvalueNode.Value.Replace("MFVideoFormat_", "");
                    }
                    else if (lvalueNode != null && lvalueNode.Value == "MFMediaType_Audio")
                    {

                        lvalueNode = lNode.SelectSingleNode("MediaTypeItem[@Name='MF_MT_AUDIO_BITS_PER_SAMPLE']/SingleValue/@Value");

                        if (lvalueNode != null)
                        {
                            mTitle = lvalueNode.Value;
                        }

                        lvalueNode = lNode.SelectSingleNode("MediaTypeItem[@Name='MF_MT_AUDIO_NUM_CHANNELS']/SingleValue/@Value");

                        if (lvalueNode != null)
                        {
                            mTitle += "x" + lvalueNode.Value;
                        }

                        lvalueNode = lNode.SelectSingleNode("MediaTypeItem[@Name='MF_MT_AUDIO_SAMPLES_PER_SECOND']/SingleValue/@Value");

                        mTitle += ", ";

                        lvalueNode = lNode.SelectSingleNode("MediaTypeItem[@Name='MF_MT_SUBTYPE']/SingleValue/@Value");

                        if (lvalueNode != null)
                        {
                            mTitle += lvalueNode.Value.Replace("MFVideoFormat_", "");
                        }
                    }


                    ContainerItem lSourceItem = new ContainerItem()
                    {
                        mFriendlyName = mTitle,// lvalueNode.Value.Replace("MFMediaType_", ""),
                        mXmlNode = lNode
                    };

                    mediaTypeComboBox.Items.Add(lSourceItem);
                }
            }
        }

        private void mediaTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string lCurrentSubType = (string)streamComboBox.SelectedItem;

            XmlNode lCurrentSourceNode = mSelectedSourceXmlNode as XmlNode;

            if (lCurrentSourceNode == null)
            {
                return;
            }

            XmlNodeList lMediaTypesNode = lCurrentSourceNode.SelectNodes("PresentationDescriptor/StreamDescriptor/MediaTypes/MediaType[MediaTypeItem[@Name='MF_MT_SUBTYPE']/SingleValue[@Value='MFVideoFormat_" + lCurrentSubType + "']]");

            if (lMediaTypesNode == null)
            {
                return;
            }

            XmlNode lStreamNode = lCurrentSourceNode.SelectSingleNode("PresentationDescriptor/StreamDescriptor");

            if (lStreamNode == null)
            {
                return;
            }

            XmlNode lValueNode = lStreamNode.SelectSingleNode("@MajorTypeGUID");

            string lXPath = "EncoderFactories/Group[@GUID='blank']/EncoderFactory";

            lXPath = lXPath.Replace("blank", lValueNode.Value);


            string lxmldoc = "";

            mCaptureManager.getCollectionOfEncoders(ref lxmldoc);

            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();

            doc.LoadXml(lxmldoc);

            XmlNodeList lEncoderNodes = doc.SelectNodes(lXPath);

            encoderComboBox.Items.Clear();

            foreach (object item in lEncoderNodes)
            {

                XmlNode lNode = (XmlNode)item;

                if (lNode != null)
                {
                    XmlNode lvalueNode = lNode.SelectSingleNode("@Title");

                    ContainerItem lSourceItem = new ContainerItem()
                    {
                        mFriendlyName = lvalueNode.Value,
                        mXmlNode = lNode
                    };

                    encoderComboBox.Items.Add(lSourceItem);
                }
            }

        }

        private void encoderComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            do
            {

                if (mEncoderControl == null)
                {
                    break;
                }

                ContainerItem lSelectedEncoderItem = (ContainerItem)encoderComboBox.SelectedItem;

                if (lSelectedEncoderItem == null)
                {
                    return;
                }

                XmlNode lselectedNode = lSelectedEncoderItem.mXmlNode;

                if (lselectedNode == null)
                {
                    break;
                }

                XmlAttribute lEncoderNameAttr = lselectedNode.Attributes["Title"];

                if (lEncoderNameAttr == null)
                {
                    break;
                }

                XmlAttribute lCLSIDEncoderAttr = lselectedNode.Attributes["CLSID"];

                if (lCLSIDEncoderAttr == null)
                {
                    break;
                }

                Guid lCLSIDEncoder;

                if (!Guid.TryParse(lCLSIDEncoderAttr.Value, out lCLSIDEncoder))
                {
                    break;
                }

                ContainerItem lSelectedSourceItem = (ContainerItem)sourceComboBox.SelectedItem;

                if (lSelectedSourceItem == null)
                {
                    return;
                }

                XmlNode lSourceNode = lSelectedSourceItem.mXmlNode;

                if (lSourceNode == null)
                {
                    return;
                }

                XmlNode lNode = lSourceNode.SelectSingleNode(
            "Source.Attributes/Attribute" +
            "[@Name='MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_SYMBOLIC_LINK' or @Name='MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_AUDCAP_SYMBOLIC_LINK']" +
            "/SingleValue/@Value");

                if (lNode == null)
                {
                    return;
                }

                string lSymbolicLink = lNode.Value;


                uint lStreamIndex = 0;

                ContainerItem lSelectedMediaTypeItem = (ContainerItem)mediaTypeComboBox.SelectedItem;

                if (lSelectedMediaTypeItem == null)
                {
                    return;
                }

                lSourceNode = lSelectedMediaTypeItem.mXmlNode;

                if (lSourceNode == null)
                {
                    return;
                }

                lNode = lSourceNode.SelectSingleNode("@Index");

                if (lNode == null)
                {
                    return;
                }

                uint lMediaTypeIndex = 0;

                if (!uint.TryParse(lNode.Value, out lMediaTypeIndex))
                {
                    return;
                }



                object lOutputMediaType;

                if (mSourceControl == null)
                {
                    return;
                }

                mSourceControl.getSourceOutputMediaType(
                    lSymbolicLink,
                    lStreamIndex,
                    lMediaTypeIndex,
                    out lOutputMediaType);

                string lMediaTypeCollection;

                if (!mEncoderControl.getMediaTypeCollectionOfEncoder(
                    lOutputMediaType,
                    lCLSIDEncoder,
                    out lMediaTypeCollection))
                {
                    break;
                }

                XmlDocument lEncoderModedoc = new XmlDocument();

                lEncoderModedoc.LoadXml(lMediaTypeCollection);

                XmlNodeList lEncoderNodes = lEncoderModedoc.SelectNodes("EncoderMediaTypes/Group");

                encoderModeComboBox.Items.Clear();

                foreach (object item in lEncoderNodes)
                {

                    lNode = (XmlNode)item;

                    if (lNode != null)
                    {
                        XmlNode lvalueNode = lNode.SelectSingleNode("@Title");

                        ContainerItem lSourceItem = new ContainerItem()
                        {
                            mFriendlyName = lvalueNode.Value,
                            mXmlNode = lNode
                        };

                        encoderModeComboBox.Items.Add(lSourceItem);
                    }
                }


            } while (false);
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void encoderModeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            XmlNode lCurrentSourceNode = mSelectedSourceXmlNode as XmlNode;

            if (lCurrentSourceNode == null)
            {
                return;
            }

            XmlNode lStreamNode = lCurrentSourceNode.SelectSingleNode("PresentationDescriptor/StreamDescriptor");

            if (lStreamNode == null)
            {
                return;
            }

            ContainerItem lSelectedEncoderModeItem = (ContainerItem)encoderModeComboBox.SelectedItem;

            if (lSelectedEncoderModeItem == null)
            {
                return;
            }

            XmlNodeList lcompressedMediaTypeNodes = lSelectedEncoderModeItem.mXmlNode.SelectNodes("MediaTypes/MediaType");

            compressedMediaTypeComboBox.Items.Clear();

            foreach (object item in lcompressedMediaTypeNodes)
            {

                XmlNode lNode = (XmlNode)item;

                if (lNode != null)
                {

                    XmlNode lvalueNode = lStreamNode.SelectSingleNode("@MajorType");

                    string mTitle = "";

                    if (lvalueNode != null && lvalueNode.Value == "MFMediaType_Video")
                    {

                        lvalueNode = lNode.SelectSingleNode("MediaTypeItem[@Name='MF_MT_FRAME_SIZE']/Value.ValueParts/ValuePart[1]/@Value");

                        mTitle = lvalueNode.Value;

                        lvalueNode = lNode.SelectSingleNode("MediaTypeItem[@Name='MF_MT_FRAME_SIZE']/Value.ValueParts/ValuePart[2 ]/@Value");

                        mTitle += "x" + lvalueNode.Value;

                        lvalueNode = lNode.SelectSingleNode("MediaTypeItem[@Name='MF_MT_FRAME_RATE']/RatioValue/@Value");

                        mTitle += ", " + lvalueNode.Value + " FPS, ";

                        lvalueNode = lNode.SelectSingleNode("MediaTypeItem[@Name='MF_MT_SUBTYPE']/SingleValue/@Value");

                        mTitle += lvalueNode.Value.Replace("MFVideoFormat_", "");
                    }
                    else if (lvalueNode != null && lvalueNode.Value == "MFMediaType_Audio")
                    {

                        lvalueNode = lNode.SelectSingleNode("MediaTypeItem[@Name='MF_MT_AUDIO_BITS_PER_SAMPLE']/SingleValue/@Value");

                        if (lvalueNode != null)
                        {
                            mTitle = lvalueNode.Value;
                        }

                        lvalueNode = lNode.SelectSingleNode("MediaTypeItem[@Name='MF_MT_AUDIO_NUM_CHANNELS']/SingleValue/@Value");

                        if (lvalueNode != null)
                        {
                            mTitle += "x" + lvalueNode.Value;
                        }

                        lvalueNode = lNode.SelectSingleNode("MediaTypeItem[@Name='MF_MT_AUDIO_SAMPLES_PER_SECOND']/SingleValue/@Value");

                        mTitle += ", ";

                        lvalueNode = lNode.SelectSingleNode("MediaTypeItem[@Name='MF_MT_SUBTYPE']/SingleValue/@Value");

                        if (lvalueNode != null)
                        {
                            mTitle += lvalueNode.Value.Replace("MFVideoFormat_", "");
                        }
                    }


                    ContainerItem lSourceItem = new ContainerItem()
                    {
                        mFriendlyName = mTitle,// lvalueNode.Value.Replace("MFMediaType_", ""),
                        mXmlNode = lNode
                    };


                    compressedMediaTypeComboBox.Items.Add(lSourceItem);
                }
            }
        }

        private void compressedMediaTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string lxmldoc = "";

            mCaptureManager.getCollectionOfSinks(ref lxmldoc);

            XmlDocument doc = new System.Xml.XmlDocument();

            doc.LoadXml(lxmldoc);

            XmlNodeList lsinkFactoryNodes = doc.SelectNodes("SinkFactories/SinkFactory");

            sinkComboBox.Items.Clear();

            foreach (object item in lsinkFactoryNodes)
            {

                XmlNode lNode = (XmlNode)item;

                if (lNode != null)
                {
                    XmlAttribute lAttr = lNode.Attributes["GUID"];

                    if (lAttr == null)
                    {
                        throw new System.Exception("GUID is empty");
                    }

                    if (lAttr.Value == "{D6E342E3-7DDD-4858-AB91-4253643864C2}")
                    {
                        XmlNode lvalueNode = lNode.SelectSingleNode("@Title");

                        ContainerItem lSourceItem = new ContainerItem()
                        {
                            mFriendlyName = lvalueNode.Value,
                            mXmlNode = lNode
                        };

                        sinkComboBox.Items.Add(lSourceItem);
                    }

                }
            }
        }

        private void sinkComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

            ContainerItem lSelectedSinkItem = (ContainerItem)sinkComboBox.SelectedItem;

            if (lSelectedSinkItem == null)
            {
                return;
            }

            XmlNodeList lContainerNodes = lSelectedSinkItem.mXmlNode.SelectNodes("Value.ValueParts/ValuePart");

            formatComboBox.Items.Clear();

            foreach (object item in lContainerNodes)
            {

                XmlNode lNode = (XmlNode)item;

                if (lNode != null)
                {
                    XmlNode lvalueNode = lNode.SelectSingleNode("@Value");

                    ContainerItem lSourceItem = new ContainerItem()
                    {
                        mFriendlyName = lvalueNode.Value,
                        mXmlNode = lNode
                    };

                    formatComboBox.Items.Add(lSourceItem);

                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            do
            {
                ContainerItem lSelectedFormatItem = (ContainerItem)formatComboBox.SelectedItem;

                if (lSelectedFormatItem == null)
                {
                    return;
                }

                XmlNode lselectedNode = lSelectedFormatItem.mXmlNode;

                if (lselectedNode == null)
                {
                    break;
                }

                XmlAttribute lSelectedAttr = lselectedNode.Attributes["Value"];

                if (lSelectedAttr == null)
                {
                    break;
                }

                String limageSourceDir = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

                SaveFileDialog lsaveFileDialog = new SaveFileDialog();

                lsaveFileDialog.InitialDirectory = limageSourceDir;

                lsaveFileDialog.DefaultExt = "." + lSelectedAttr.Value.ToLower();

                lsaveFileDialog.AddExtension = true;

                lsaveFileDialog.CheckFileExists = false;

                lsaveFileDialog.Filter = "Media file (*." + lSelectedAttr.Value.ToLower() + ")|*." + lSelectedAttr.Value.ToLower();

                DialogResult lresult = lsaveFileDialog.ShowDialog();

                if (lresult != DialogResult.OK)
                {
                    break;
                }

                mDo.Enabled = true;

                lSelectedAttr = lselectedNode.Attributes["GUID"];

                if (lSelectedAttr == null)
                {
                    break;
                }

                IFileSinkFactory lFileSinkFactory;

                mSinkControl.createSinkFactory(
                    Guid.Parse(lSelectedAttr.Value),
                    out lFileSinkFactory);

                mSink = new FileSink(lFileSinkFactory);

                mSink.setOptions(lsaveFileDialog.FileName);

            }
            while (false);
        }

        private void mDo_Click(object sender, EventArgs e)
        {


            if (mSession != null)
            {
                mSession.closeSession();

                mSession = null;

                mDo.Text = "Stopped";

                return;
            }

            if (mSink == null)
            {
                return;
            }

            ContainerItem lSelectedSourceItem = (ContainerItem)sourceComboBox.SelectedItem;

            if (lSelectedSourceItem == null)
            {
                return;
            }

            XmlNode lSourceNode = lSelectedSourceItem.mXmlNode;

            if (lSourceNode == null)
            {
                return;
            }

            XmlNode lNode = lSourceNode.SelectSingleNode(
                "Source.Attributes/Attribute" +
                "[@Name='MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_SYMBOLIC_LINK' or @Name='MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_AUDCAP_SYMBOLIC_LINK']" +
                "/SingleValue/@Value");

            if (lNode == null)
            {
                return;
            }

            string lSymbolicLink = lNode.Value;

            uint lStreamIndex = 0;

            ContainerItem lSelectedMediaTypeItem = (ContainerItem)mediaTypeComboBox.SelectedItem;

            if (lSelectedMediaTypeItem == null)
            {
                return;
            }

            lSourceNode = lSelectedMediaTypeItem.mXmlNode;

            if (lSourceNode == null)
            {
                return;
            }

            lNode = lSourceNode.SelectSingleNode("@Index");

            if (lNode == null)
            {
                return;
            }

            uint lMediaTypeIndex = 0;

            if (!uint.TryParse(lNode.Value, out lMediaTypeIndex))
            {
                return;
            }

            object lOutputMediaType;

            mSourceControl.getSourceOutputMediaType(
                        lSymbolicLink,
                        lStreamIndex,
                        lMediaTypeIndex,
                        out lOutputMediaType);


            ContainerItem lSelectedEncoderItem = (ContainerItem)encoderComboBox.SelectedItem;

            if (lSelectedEncoderItem == null)
            {
                return;
            }

            XmlNode lselectedNode = lSelectedEncoderItem.mXmlNode;

            if (lselectedNode == null)
            {
                return;
            }

            XmlAttribute lEncoderNameAttr = lselectedNode.Attributes["Title"];

            if (lEncoderNameAttr == null)
            {
                return;
            }

            XmlAttribute lCLSIDEncoderAttr = lselectedNode.Attributes["CLSID"];

            if (lCLSIDEncoderAttr == null)
            {
                return;
            }

            Guid lCLSIDEncoder;

            if (!Guid.TryParse(lCLSIDEncoderAttr.Value, out lCLSIDEncoder))
            {
                return;
            }

            IEncoderNodeFactory lEncoderNodeFactory;

            mEncoderControl.createEncoderNodeFactory(
                lCLSIDEncoder,
                out lEncoderNodeFactory);




            ContainerItem lSelectedEncoderModeItem = (ContainerItem)encoderModeComboBox.SelectedItem;

            if (lSelectedEncoderModeItem == null)
            {
                return;
            }

            lselectedNode = lSelectedEncoderModeItem.mXmlNode;

            if (lselectedNode == null)
            {
                return;
            }

            XmlAttribute lGUIDEncodingModeAttr = lselectedNode.Attributes["GUID"];

            if (lGUIDEncodingModeAttr == null)
            {
                return;
            }

            Guid lGUIDEncodingMode;

            if (!Guid.TryParse(lGUIDEncodingModeAttr.Value, out lGUIDEncodingMode))
            {
                return;
            }

            if (compressedMediaTypeComboBox.SelectedIndex < 0)
            {
                return;
            }

            object lCompressedMediaType;

            lEncoderNodeFactory.createCompressedMediaType(
                lOutputMediaType,
                lGUIDEncodingMode,
                70,
                (uint)compressedMediaTypeComboBox.SelectedIndex,
                out lCompressedMediaType);

            object lOutputNode = mSink.getOutputNode(lCompressedMediaType);

            IEncoderNodeFactory lIEncoderNodeFactory;

            mEncoderControl.createEncoderNodeFactory(
                lCLSIDEncoder,
                out lIEncoderNodeFactory);

            object lEncoderNode;

            lIEncoderNodeFactory.createEncoderNode(
                lOutputMediaType,
                lGUIDEncodingMode,
                70,
                (uint)compressedMediaTypeComboBox.SelectedIndex,
                lOutputNode,
                out lEncoderNode);

            object lSourceMediaNode;


            mSourceControl.createSourceNode(
                        lSymbolicLink,
                        lStreamIndex,
                        lMediaTypeIndex,
                        lEncoderNode,
                        out lSourceMediaNode);

            List<object> lSourcesList = new List<object>();

            lSourcesList.Add(lSourceMediaNode);

            mSession = mISessionControl.createSession(lSourcesList.ToArray());


            if (mSession != null)
            {
                mSession.startSession(0, Guid.Empty);
            }

            mDo.Text = "Record is executed!!!";
        }
    }
}
