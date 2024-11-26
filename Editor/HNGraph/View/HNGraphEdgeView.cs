using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace HN.Graph.Editor
{
    public class HNGraphEdgeView : Edge
    {
        public HNGraphPortView OutputPortView
        {
            get { return (HNGraphPortView)output; }
            set { output = value; }
        }

        public HNGraphPortView InputPortView
        {
            get { return (HNGraphPortView)input; }
            set { input = value; }
        }

        public HNGraphConnectionView ConnectionView
        {
            get { return connectionView; }
            set { connectionView = value; }
        }

        // public int Index => index;


        private HNGraphConnectionView connectionView;

        private HNGraphView graphView;
        // private int index = 0;
        

        public HNGraphEdgeView(HNGraphView graphView)
        {
            this.graphView = graphView;

            RegisterCallback<MouseDownEvent>(OnMouseDown);
        }

        public void Initialize(HNGraphConnectionView connectionView, HNGraphPortView output, HNGraphPortView input)
        {
            ConnectionView = connectionView;
            // this.index = index;
            OutputPortView = output;
            ConnectOutput(OutputPortView);
            InputPortView = input;
            ConnectInput(InputPortView);
        }

        public void ConnectOutput(HNGraphPortView outputPortView)
        {
            OutputPortView = outputPortView;
            OutputPortView.ConnectToEdge(this);
        }

        public void ConnectInput(HNGraphPortView inputPortView)
        {
            InputPortView = inputPortView;
            inputPortView.ConnectToEdge(this);
        }    

        public void DisconnectOutput()
        {
            if(OutputPortView != null)
            {
                OutputPortView.DisconnectFromEdge(this);
                OutputPortView = null;
            }
        }

        public void DisconnectInput()
        {
            if(InputPortView != null)
            {
                InputPortView.DisconnectFromEdge(this);
                InputPortView = null;
            }
        }

        public void DisconnectAll()
        {
            DisconnectOutput();
            DisconnectInput();
        }

        public HNGraphPortView GetAnotherPort(HNGraphPortView port)
        {
            if(OutputPortView == port)
            {
                return InputPortView;
            }
            else if(InputPortView == port)
            {
                return OutputPortView;
            }
            else
            {
                return null;
            }
        }

        private void OnMouseDown(MouseDownEvent e)
        {
            if(e.altKey)
            {
                graphView.AddRelayNode(this, e.localMousePosition);
            }
        }
    }
}
