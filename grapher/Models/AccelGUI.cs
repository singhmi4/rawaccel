﻿using grapher.Models.Calculations;
using grapher.Models.Mouse;
using grapher.Models.Options;
using grapher.Models.Serialized;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace grapher
{
    public class AccelGUI
    {

        #region constructors

        public AccelGUI(
            RawAcceleration accelForm,
            AccelCalculator accelCalculator,
            AccelCharts accelCharts,
            SettingsManager settings,
            AccelOptions accelOptions,
            OptionXY sensitivity,
            Option rotation,
            OptionXY weight,
            CapOptions cap,
            OffsetOptions offset,
            Option acceleration,
            Option limtOrExp,
            Option midpoint,
            Button writeButton,
            Label mouseMoveLabel,
            ToolStripMenuItem scaleMenuItem,
            ToolStripMenuItem autoWriteMenuItem)
        {
            AccelForm = accelForm;
            AccelCalculator = accelCalculator;
            AccelCharts = accelCharts;
            AccelerationOptions = accelOptions;
            Sensitivity = sensitivity;
            Rotation = rotation;
            Weight = weight;
            Cap = cap;
            Offset = offset;
            Acceleration = acceleration;
            LimitOrExponent = limtOrExp;
            Midpoint = midpoint;
            WriteButton = writeButton;
            ScaleMenuItem = scaleMenuItem;
            Settings = settings;
            Settings.Startup();
            UpdateGraph();

            MouseWatcher = new MouseWatcher(AccelForm, mouseMoveLabel, AccelCharts);

            ScaleMenuItem.Click += new System.EventHandler(OnScaleMenuItemClick);
        }

        #endregion constructors

        #region properties

        public RawAcceleration AccelForm { get; }

        public AccelCalculator AccelCalculator { get; }

        public AccelCharts AccelCharts { get; }

        public SettingsManager Settings { get; }

        public AccelOptions AccelerationOptions { get; }

        public OptionXY Sensitivity { get; }

        public Option Rotation { get; }

        public OptionXY Weight { get; }

        public CapOptions Cap { get; }

        public OffsetOptions Offset { get; }

        public Option Acceleration { get; }

        public Option LimitOrExponent { get; }

        public Option Midpoint { get; }

        public Button WriteButton { get; }

        public MouseWatcher MouseWatcher { get; }

        public ToolStripMenuItem ScaleMenuItem { get; }

        #endregion properties

        #region methods

        public void UpdateActiveSettingsFromFields()
        {
            var settings = new DriverSettings
            {
                rotation = Rotation.Field.Data,
                sensitivity = new Vec2<double>
                {
                    x = Sensitivity.Fields.X,
                    y = Sensitivity.Fields.Y
                },
                combineMagnitudes = true,
                modes = new Vec2<AccelMode>
                {
                    x = (AccelMode)AccelerationOptions.AccelerationIndex
                },
                args = new Vec2<AccelArgs>
                {
                    x = new AccelArgs
                    {
                        offset = Offset.Offset,
                        legacy_offset = Offset.LegacyOffset,
                        weight = Weight.Fields.X,
                        gainCap = Cap.VelocityGainCap,
                        scaleCap = Cap.SensitivityCapX,
                        accel = Acceleration.Field.Data,
                        rate = Acceleration.Field.Data,
                        powerScale = Acceleration.Field.Data,
                        limit = LimitOrExponent.Field.Data,
                        exponent = LimitOrExponent.Field.Data,
                        powerExponent = LimitOrExponent.Field.Data,
                        midpoint = Midpoint.Field.Data
                    }
                },
                minimumTime = .4
            };

            Settings.UpdateActiveSettings(settings, () =>
            {
                AccelForm.Invoke((MethodInvoker)delegate
                {
                    UpdateGraph();
                });
            });
            
        }

        public void UpdateGraph()
        {
            AccelCalculator.Calculate(
                AccelCharts.AccelData, 
                Settings.ActiveAccel, 
                Settings.RawAccelSettings.AccelerationSettings);
            AccelCharts.Bind();
            UpdateActiveValueLabels();
        }

        public void UpdateActiveValueLabels()
        {
            var settings = Settings.RawAccelSettings.AccelerationSettings;
            
            Sensitivity.SetActiveValues(settings.sensitivity.x, settings.sensitivity.y);
            Rotation.SetActiveValue(settings.rotation);
            AccelerationOptions.SetActiveValue((int)settings.modes.x);
            Offset.SetActiveValue(settings.args.x.offset, settings.args.y.offset);
            Weight.SetActiveValues(settings.args.x.weight, settings.args.x.weight);
            Acceleration.SetActiveValue(settings.args.x.accel); // rate, powerscale
            LimitOrExponent.SetActiveValue(settings.args.x.limit); //exp, powerexp
            Midpoint.SetActiveValue(settings.args.x.midpoint);
            //Cap.SetActiveValues(Settings.ActiveAccel.GainCap, Settings.ActiveAccel.CapX, Settings.ActiveAccel.CapY, Settings.ActiveAccel.GainCapEnabled);
        }

        private void OnScaleMenuItemClick(object sender, EventArgs e)
        {
            UpdateGraph();
        }
        #endregion methods
    }

}
