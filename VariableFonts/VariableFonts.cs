using System;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using Tizen.NUI;
using Tizen.NUI.Components;
using Tizen.NUI.BaseComponents;

namespace VariableFonts
{
    class Program : NUIApplication
    {
        const int WINDOW_WIDTH = 1920;
        const int WINDOW_HEIGHT = 1080;
        const int LABEL_WIDTH = 600;
        const int LABEL_HEIGHT = 200;
        const int LABEL_POINT_SIZE = 48;
        const int SLIDER_WIDTH = 600;
        const int LR_WIDTH = 100;
        const int VARIABLE_CNT = 3;
        Window window;
        View mainView;
        TextLabel label;
        List<Slider> sliderList = new List<Slider>();
        List<TextLabel> rightTextLabel = new List<TextLabel>();
        Animation[] animationList = new Animation[VARIABLE_CNT];
        string[] variableName = new string[]{
            "wght",
            "wdth",
            "slnt",
            "XOPQ",
            "YOPQ",
        };

        float[] variableMin = new float[]{
            100,
            25,
            -10,
            27,
            25,
        };

        float[] variableMax = new float[]{
            1000,
            151,
            0,
            175,
            135,
        };

        float[] variableDefault = new float[]{
            400,
            100,
            0,
            96,
            79,
        };

        int[] variablePropertyIndex = new int[VARIABLE_CNT];

        bool isUnderline = false;
        bool isStrikeThrough = false;
        bool isShadow = false;
        bool isOutline = false;
        bool isCutout = false;
        bool isEllipsis = false;
        bool isAutoScroll = false;
        bool isAsync = false;

        private void OnValueChanged(object sender, SliderValueChangedEventArgs args)
        {
            Slider slider = (Slider)sender;
            int sliderIdx = Int32.Parse(slider.Name);
            float value = (float)Math.Round((double)slider.CurrentValue);
            label.SetProperty(variablePropertyIndex[sliderIdx], new PropertyValue(value));
            rightTextLabel[sliderIdx].Text = value.ToString();
        }

        private void OnStartButtonClicked(object sender, ClickedEventArgs args)
        {
            Button button = (Button)sender;
            int buttonIdx = Int32.Parse(button.Name);

            Tizen.Log.Error("VariableFonts", "Idx: " + buttonIdx);

            Animation anim = new Animation(1000);
            anim.AnimateTo(label, variableName[buttonIdx], variableMax[buttonIdx]);
            anim.Looping = true;
            anim.LoopingMode = Animation.LoopingModes.AutoReverse;
            animationList[buttonIdx] = anim;

            label.SetProperty(variablePropertyIndex[buttonIdx], new PropertyValue(variableMin[buttonIdx]));
            anim.Play();
        }

        private void OnEndButtonClicked(object sender, ClickedEventArgs args)
        {
            Button button = (Button)sender;
            int buttonIdx = Int32.Parse(button.Name);

            if(animationList[buttonIdx])
            {
                animationList[buttonIdx].Stop();
            }
        }

        ImageView makeImageView(string url)
        {
            var image = new ImageView
            {
                ResourceUrl = url,
                //Size = new Tizen.NUI.Size(1000, 500),
                HeightResizePolicy = ResizePolicyType.FillToParent,
                WidthResizePolicy = ResizePolicyType.FillToParent,
            };

            return image;
        }

        protected override void OnCreate()
        {
            string resourcePath = Tizen.Applications.Application.Current.DirectoryInfo.Resource;
            FontClient.Instance.AddCustomFontDirectory(resourcePath + "/fonts");

            window = NUIApplication.GetDefaultWindow();
            window.WindowSize = new Size(WINDOW_WIDTH, WINDOW_HEIGHT);
            window.BackgroundColor = Color.White;

            mainView = new View()
            {
                Layout = new LinearLayout
                {
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                },
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.WrapContent,
                BackgroundColor = Color.Transparent,
                Margin = new Extents(0, 0, 6, 6),
            };

            window.Add(mainView);

            var imageView = makeImageView(resourcePath + "/images/rainbow.jpg");

            var labelView = new View
            {
                Size2D = new Size2D(LABEL_WIDTH, LABEL_HEIGHT),
                PositionUsesPivotPoint = true,
                PivotPoint = PivotPoint.TopLeft,
                ParentOrigin = ParentOrigin.TopLeft,
            };

            label = new TextLabel();
            label.Size2D = new Size2D(LABEL_WIDTH, LABEL_HEIGHT);
            label.TextColor = Color.Black;
            label.BackgroundColor = Color.White;
            label.PointSize = LABEL_POINT_SIZE;
            label.FontFamily = "RobotoFlex";
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.VerticalAlignment = VerticalAlignment.Center;
            label.Text = "ABCabc123";
            label.AutoScrollStopMode = AutoScrollStopMode.Immediate;

            labelView.Add(imageView);
            labelView.Add(label);

            mainView.Add(labelView);

            for(int i=0;i<VARIABLE_CNT;i++)
            {
                var propretyIdx = label.RegisterFontVariationProperty(variableName[i]);
                variablePropertyIndex[i] = propretyIdx;
                label.SetProperty(propretyIdx, new PropertyValue(variableDefault[i]));

                View subView = new View()
                {
                    Layout = new LinearLayout
                    {
                        LinearOrientation = LinearLayout.Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                    },
                };

                TextLabel leftLabel = new TextLabel()
                {
                    Text = variableName[i],
                    SizeWidth = LR_WIDTH,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };

                TextLabel rightLabel = new TextLabel()
                {
                    Text = "" + variableDefault[i],
                    SizeWidth = LR_WIDTH,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                rightTextLabel.Add(rightLabel);

                Slider slider = new Slider()
                {
                    Name = i.ToString(),
                    MinValue = variableMin[i],
                    MaxValue = variableMax[i],
                    CurrentValue = variableDefault[i],
                    IsValueShown = true,
                    TrackThickness = 10,
                    SlidedTrackColor = Color.Orange,
                    BgTrackColor = Color.Grey,
                    WidthSpecification = SLIDER_WIDTH,
                };
                slider.ValueChanged += OnValueChanged;

                var startButton = new Button()
                {
                    Name = i.ToString(),
                    Text = "Start",
                    BackgroundColor = Color.Red,
                };
                startButton.Clicked += OnStartButtonClicked;

                var endButton = new Button()
                {
                    Name = i.ToString(),
                    Text = "End",
                    BackgroundColor = Color.Blue,
                };
                endButton.Clicked += OnEndButtonClicked;

                subView.Add(leftLabel);
                subView.Add(slider);
                subView.Add(rightLabel);
                subView.Add(startButton);
                subView.Add(endButton);

                mainView.Add(subView);
                sliderList.Add(slider);
            }

            View subView2 = new View()
            {
                Layout = new GridLayout()
                {
                    Rows = 2,
                    GridOrientation = GridLayout.Orientation.Vertical,
                },
            };
            mainView.Add(subView2);

            var underlineButton = new Button()
            {
                Text = "Underline",
            };
            underlineButton.Clicked += (object sender, ClickedEventArgs e) =>
            {
                if(!isUnderline)
                {
                    label.UnderlineEnabled = true;
                    label.UnderlineColor = Color.Red;
                    isUnderline = true;
                }
                else
                {
                    label.UnderlineEnabled = false;
                    isUnderline = false;
                }
            };
            subView2.Add(underlineButton);

            var shadowButton = new Button()
            {
                Text = "Shadow",
            };
            shadowButton.Clicked += (object sender, ClickedEventArgs e) =>
            {
                if(!isShadow)
                {
                    PropertyMap ShadowMap = new PropertyMap();
                    ShadowMap.Add("color", new PropertyValue(Color.Blue));
                    ShadowMap.Add("offset", new PropertyValue(new Vector2(3.0f, 5.0f)));
                    ShadowMap.Add("blurRadius", new PropertyValue(2.0f));

                    label.Shadow = ShadowMap;
                    isShadow = true;
                }
                else
                {
                    label.Shadow = new PropertyMap();
                    isShadow = false;
                }
            };
            subView2.Add(shadowButton);

            var outlineButton = new Button()
            {
                Text = "Outline",
            };
            outlineButton.Clicked += (object sender, ClickedEventArgs e) =>
            {
                if(!isOutline)
                {
                    PropertyMap OutlineMap = new PropertyMap();
                    OutlineMap.Add("width", new PropertyValue(5.0f));
                    OutlineMap.Add("color", new PropertyValue(Color.Orange));
                    OutlineMap.Add("offset", new PropertyValue(new Vector2(0, 0)));
                    OutlineMap.Add("blurRadius", new PropertyValue(1.0f));

                    label.Outline = OutlineMap;
                    isOutline = true;
                }
                else
                {
                    label.Outline = new PropertyMap();
                    isOutline = false;
                }
            };
            subView2.Add(outlineButton);

            var cutoutButton = new Button()
            {
                Text = "Cutout",
            };
            cutoutButton.Clicked += (object sender, ClickedEventArgs e) =>
            {
                if(!isCutout)
                {
                    label.Cutout = true;
                    label.TextColor = new Color(0, 0, 0, 0);
                    isCutout = true;
                }
                else
                {
                    label.Cutout = false;
                    label.TextColor = Color.Black;
                    isCutout = false;
                }
            };
            subView2.Add(cutoutButton);

            var ellipsisButton = new Button()
            {
                Text = "Ellipsis",
            };
            ellipsisButton.Clicked += (object sender, ClickedEventArgs e) =>
            {
                if(!isEllipsis)
                {
                    label.Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Morbi at facilisis orci, nec vestibulum diam. Curabitur nibh orci, fermentum nec enim eu, efficitur ullamcorper orci. Nullam vitae metus vel dui rhoncus ultricies nec vitae risus. Pellentesque vitae rutrum est. Nam sed eros sapien. Nunc consequat egestas odio at imperdiet. Mauris ultricies varius arcu, vel eleifend lectus mollis et. In porttitor mattis suscipit. Phasellus luctus nec dui at tincidunt. In interdum nisi nunc, ac sagittis sem imperdiet in. Curabitur vulputate nulla nisi, eget tempor tortor finibus id. Donec justo tortor, scelerisque nec lorem vitae, laoreet pellentesque sem. In placerat ipsum lobortis ante iaculis, eu rhoncus sapien commodo. Nunc auctor metus vitae nulla laoreet auctor. Suspendisse ultricies nec ipsum egestas scelerisque. Curabitur elit enim, tempor id dui non, venenatis scelerisque odio. Duis malesuada elit semper venenatis facilisis. Praesent fermentum eleifend justo vitae laoreet. Sed tristique pretium gravida. In eget commodo felis, porta volutpat dolor. Sed mattis condimentum enim, quis fermentum mi dictum id. Pellentesque suscipit massa elit, eget mollis dui euismod a. Maecenas mattis lacus at leo pellentesque, ut luctus nulla ultrices. Proin posuere, urna id rhoncus vulputate, lorem nisi cursus purus, vel luctus purus orci ac dui. Aliquam vitae lectus augue. Praesent consectetur odio vel dolor mollis, vitae varius odio pretium. Ut a imperdiet ligula. Aenean vel est sit amet nisi dictum pharetra. Orci varius natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec non massa nisl. Aliquam sagittis gravida metus, et pulvinar quam semper ut. Integer ut orci libero. In eu molestie turpis, et tempus libero. Vestibulum eget aliquam felis. Sed nec neque lacus. Donec a enim in lacus viverra fringilla. Ut placerat venenatis blandit. In lacinia consectetur ex. Curabitur scelerisque neque sed ante viverra posuere. Vestibulum congue lacus non odio bibendum, id elementum mi vulputate. Vestibulum finibus eget ex id pulvinar. Maecenas imperdiet ultrices ullamcorper. Donec in tincidunt erat. Pellentesque lobortis lacinia dapibus. Phasellus aliquam velit diam, ut mollis nibh tempus quis. Vivamus ornare ipsum metus, ut dictum sapien feugiat id. Curabitur viverra ex mauris, ac suscipit urna mollis a. Quisque malesuada interdum risus nec imperdiet. Sed ultrices, nisl ut porta ornare, dui eros mattis velit, tincidunt semper felis ligula vel ligula. Integer ullamcorper massa id maximus hendrerit. Nulla ultricies rhoncus nisl, sit amet placerat erat condimentum eu. Morbi non congue lectus. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nam id auctor ipsum. Aliquam euismod nunc sit amet nisi euismod placerat. Vestibulum tempus nisi eu nibh pulvinar, eget bibendum augue gravida. Quisque nec orci nec turpis fringilla tempor et nec mauris. Aliquam ultricies sodales augue nec tincidunt. Duis non tempus risus. Morbi sed suscipit mi. Nulla pellentesque ante ut velit laoreet molestie. ";
                    isEllipsis = true;
                }
                else
                {
                    label.Text = "ABCabc123";
                    isEllipsis = false;
                }
            };
            subView2.Add(ellipsisButton);

            var autoScrollButton = new Button()
            {
                Text = "Auto Scroll",
            };
            autoScrollButton.Clicked += (object sender, ClickedEventArgs e) =>
            {
                if(!isAutoScroll)
                {
                    label.EnableAutoScroll = true;
                    isAutoScroll = true;
                }
                else
                {
                    label.EnableAutoScroll = false;
                    isAutoScroll = false;
                }
            };
            subView2.Add(autoScrollButton);

/*
            var asyncButton = new Button()
            {
                Text = "Async",
            };
            asyncButton.Clicked += (object sender, ClickedEventArgs e) =>
            {
                if(!isAsync)
                {
                    label.RenderMode = TextRenderMode.AsyncAuto;
                    isAsync = true;
                }
                else
                {
                    label.RenderMode = TextRenderMode.Sync;
                    isAsync = false;
                }
            };
            subView2.Add(asyncButton);
*/
        }

        static void Main(string[] args)
        {
            var app = new Program();
            app.Run(args);
        }
    }
}
