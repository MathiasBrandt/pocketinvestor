
namespace Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;

    using Stock;

    /// <summary>
    /// A single graphical component displaying a specific stock group and its data.
    /// </summary>
    public class StockGroupComponent
    {
        #region Constants and Fields

        private readonly List<StockDataPoint> _dataPoints;

        private readonly int _investmentPercentage;

        private readonly InvestmentScreen _investmentScreen;

        private int _maxPossibleSliderValue;

        private Slider _slider;

        private String _stockGroupName;

        private Image _arrowImage;

        #endregion

        #region Constructors and Destructors

        public StockGroupComponent(InvestmentScreen invScreen, String stockGroupName)
        {
            Contract.Requires(!ReferenceEquals(stockGroupName, null));

            _investmentScreen = invScreen;
            _stockGroupName = stockGroupName;
            _investmentPercentage =  Communication.Instance.Investments(stockGroupName);
            _dataPoints = Communication.Instance.StockGroupData(_stockGroupName);

            CreatePanel();
            CreateGraphComponent();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Returns the investment screen on which this component is displayed.
        /// </summary>
        public InvestmentScreen InvestmentScreen
        {
            get
            {
                return _investmentScreen;
            }
        }

        /// <summary>
        /// Gets or sets the current value of the slider.
        /// </summary>
        public int SliderValue
        {
            [Pure]
            get
            {
                return (int) _slider.Value;
            }
            set
            {
                Contract.Requires(value >= 0);
                Contract.Requires(value <= 100);
                Contract.Ensures(SliderValue == value);
                Contract.Ensures(InvestmentScreen.SumOfAllSliders == Contract.OldValue(InvestmentScreen.SumOfAllSliders) + (value - Contract.OldValue(SliderValue)));

                _slider.Value = value;
            }
        }

        /// <summary>
        /// Gets the stock group name of this component.
        /// </summary>
        public String StockGroupName
        {
            get
            {
                return _stockGroupName;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Updates the number displaying the difference between the original and the new value of the investment percentage.
        /// </summary>
        public void UpdateGroupInfoHeader()
        {
            // Update text in GROUP_INFO_HEADER
            StackPanel topPanel = _investmentScreen.outerPanel.FindName("topPanel_" + _stockGroupName) as StackPanel;
            TextBlock groupInfoHeaderTb = topPanel.Children[1] as TextBlock;
            int difference = (Convert.ToInt32(SliderValue) - Communication.Instance.Investments(_stockGroupName));
            String differenceString = difference > 0 ? "+" + difference : "" + difference;
            groupInfoHeaderTb.Text = "Group: " + _stockGroupName + "\r" + "Investments: " +
                                     Convert.ToInt32(SliderValue) + "% (" +
                                     differenceString + ")";
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates the slider and the stock trend graph.
        /// </summary>
        private void CreateGraphComponent()
        {
            StackPanel graphPanel = _investmentScreen.outerPanel.FindName("graphPanel_" + _stockGroupName) as StackPanel;

            // Create the slider
            _slider = new Slider
            {
                Name = "slider_" + _stockGroupName,
                Height = 84,
                Width = graphPanel.Width - 100,
                Maximum = 100,
                Minimum = 0,
                Value = _investmentPercentage
            };
            _slider.GotFocus += SetMaxPossibleSliderValue;
            _slider.ValueChanged += UpdateSliderValue;

            // Create the graph
            Canvas c = new Canvas();
            c.Height = 100;
            c.Width = graphPanel.Width - 100;

            Polyline trend = new Polyline();
            trend.Stroke = new SolidColorBrush(Color.FromArgb(255, 27, 162, 224));
            trend.StrokeThickness = 4;

            double xOffset = DateTime.Now.AddYears(-5).Ticks / 10000000;
            double xScale = c.Width / ((DateTime.Now.AddYears(5).Ticks / 10000000) - xOffset);
            double yOffset = 0;
            double yScale = c.Height / 200.0;

            PointCollection trendPoints = new PointCollection();
            foreach (StockDataPoint dataPoint in _dataPoints)
            {
                double x = (dataPoint.TimeStamp - xOffset) * xScale;
                double y = (dataPoint.Value - yOffset) * yScale;
                trendPoints.Add(new Point(x, y));
            }

            trend.Points = trendPoints;
            c.Children.Add(trend);

            graphPanel.Children.Add(_slider);
            graphPanel.Children.Add(c);
        }

        /// <summary>
        /// Creates the top panel of the component.
        /// </summary>
        private void CreatePanel()
        {
            StackPanel topPanel = new StackPanel
                {
                    Name = "topPanel_" + _stockGroupName,
                    Width = 470,
                    Height = 80,
                    Margin = new Thickness(0.0, 0.0, 0.0, 0.0),
                    Orientation = System.Windows.Controls.Orientation.Horizontal,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
            topPanel.Tap += GraphPanelExpandCollapse;

            _arrowImage = new Image
                {
                    Name = "image_" + _stockGroupName,
                    Height = 80,
                    Width = 80,
                    Stretch = Stretch.None,
                    Source = _investmentScreen.border_investmentGroups.Resources["img_arrow"] as BitmapImage
                };

            TextBlock groupInfoHeader = new TextBlock       // GROUP_INFO_HEADER
                {
                    Height = 80,
                    Width = 390,
                    Margin = new Thickness(0.0, 12.5, 0.0, 0.0),
                    Text = "Group: " + _stockGroupName + "\r" +
                           "Investments: " + _investmentPercentage + "% (0)"
                };

            topPanel.Children.Add(_arrowImage);
            topPanel.Children.Add(groupInfoHeader);

            StackPanel graphComponentPanel = new StackPanel
                {
                    Name = "graphPanel_" + _stockGroupName,
                    Height = 200,
                    Width = 470,
                    Margin = new Thickness(0.0, 0.0, 0.0, 0.0),
                    Visibility = Visibility.Collapsed
                };

            _investmentScreen.outerPanel.Children.Add(topPanel);
            _investmentScreen.outerPanel.Children.Add(graphComponentPanel);
        }

        /// <summary>
        /// Expands or collapses the graph panel based on its current state.
        /// </summary>
        private void GraphPanelExpandCollapse(object sender, RoutedEventArgs e)
        {
            
            StackPanel sp = _investmentScreen.border_investmentGroups.FindName("graphPanel_" + _stockGroupName) as StackPanel;
            if (sp.Visibility == Visibility.Collapsed)
            {
                sp.Visibility = Visibility.Visible;

                Duration duration = new Duration(TimeSpan.FromSeconds(0.275));
                Storyboard sb = new Storyboard();
                sb.Duration = duration;

                DoubleAnimation da = new DoubleAnimation();
                da.Duration = duration;

                sb.Children.Add(da);

                RotateTransform rt = new RotateTransform();

                Storyboard.SetTarget(da, rt);
                Storyboard.SetTargetProperty(da, new PropertyPath("Angle"));
                da.From = 0;
                da.To = 90;

                _arrowImage.RenderTransform = rt;
                _arrowImage.RenderTransformOrigin = new Point(0.3, 0.3);

                sb.Begin();

                /* Animation
                Storyboard sbShow = border_investmentGroups.Resources["anim_showGroup"] as Storyboard;
                sbShow.Stop();

                Storyboard.SetTarget(anim_showGroup, sp);
                sbShow.Begin();

                img.Source = border_investmentGroups.Resources["img_arrowCollapse"] as BitmapImage;
                */
            }
            else
            {
                sp.Visibility = Visibility.Collapsed;

                Duration duration = new Duration(TimeSpan.FromSeconds(0.275));
                Storyboard sb = new Storyboard();
                sb.Duration = duration;

                DoubleAnimation da = new DoubleAnimation();
                da.Duration = duration;

                sb.Children.Add(da);

                RotateTransform rt = new RotateTransform();

                Storyboard.SetTarget(da, rt);
                Storyboard.SetTargetProperty(da, new PropertyPath("Angle"));
                da.From = 90;
                da.To = 0;

                _arrowImage.RenderTransform = rt;
                _arrowImage.RenderTransformOrigin = new Point(0.3, 0.3);

                sb.Begin();

                //img.Source = _investmentScreen.border_investmentGroups.Resources["img_arrowExpand"] as BitmapImage;

                /* Animation
                Storyboard sbHide = border_investmentGroups.Resources["anim_hideGroup"] as Storyboard;
                sbHide.Stop();

                Storyboard.SetTarget(anim_hideGroup, sp);
                sbHide.Begin();

                img.Source = border_investmentGroups.Resources["img_arrowExpand"] as BitmapImage;
                */
            }
        }

        /// <summary>
        /// Sets the max possible value of this slider based on the sum of all sliders.
        /// </summary>
        private void SetMaxPossibleSliderValue(Object sender, RoutedEventArgs args)
        {
            _maxPossibleSliderValue = 100 - _investmentScreen.SumOfAllSliders + SliderValue;
        }

        /// <summary>
        /// Called when the user changes the slider value.
        /// </summary>
        private void UpdateSliderValue(Object sender, RoutedPropertyChangedEventArgs<Double> args)
        {
            // If the sum of all sliders exceed 100% ...
            if (args.NewValue > _maxPossibleSliderValue)
            {
                _slider.Value = _maxPossibleSliderValue;
            }

            _investmentScreen.UpdateSubmitButtonStatus();

            UpdateGroupInfoHeader();

            _investmentScreen.UpdateControlPanelText();
        }

        #endregion
    }
}
