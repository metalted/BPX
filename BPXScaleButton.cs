using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using System.Globalization;

namespace BPX
{
    public class BPXScaleButton : MonoBehaviour
    {
        private TextMeshProUGUI buttonText;
        private int selectedScaleValueIndex = 0;
        private float[] latestScalingValues;
        private float defaultScalingValue = 10f;
        private float currentScalingValue = 10f;

        public void Start()
        {
            latestScalingValues = BPXConfiguration.GetScalingValues();

            buttonText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            
            //Change the label text for the scaling button.
            transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "S";

            Set();
        }

        public void OnClick()
        {
            latestScalingValues = BPXConfiguration.GetScalingValues();

            if (BPXManager.central.input.MultiSelect.buttonHeld)
            {
                //Go to the previous scale value.
                selectedScaleValueIndex--;
            }
            else
            {
                //Go to the next scale value.
                selectedScaleValueIndex++;
            }

            Set();
        }

        public float GetCurrentValue()
        {
            return currentScalingValue;
        }

        private void SetText(string value)
        {
            buttonText.text = value;
        }

        private void Set()
        {
            if (latestScalingValues.Length == 0)
            {
                currentScalingValue = defaultScalingValue;
            }
            else
            {
                if (selectedScaleValueIndex < 0)
                {
                    selectedScaleValueIndex = latestScalingValues.Length - 1;
                }
                else if (selectedScaleValueIndex >= latestScalingValues.Length)
                {
                    selectedScaleValueIndex = 0;
                }

                currentScalingValue = latestScalingValues[selectedScaleValueIndex];
            }
            
            SetText(currentScalingValue.ToString(CultureInfo.InvariantCulture));
        }
    }
}
