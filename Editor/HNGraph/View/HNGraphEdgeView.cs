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
        public HNGraphEdge EdgeData => edgeData;

        public HNGraphBasePortView OutputPortView => output as HNGraphBasePortView;

        public HNGraphBasePortView InputPortView => input as HNGraphBasePortView;


        private HNGraphEdge edgeData;
        private HNGraphView graphView;
        

        public HNGraphEdgeView(HNGraphView graphView)
        {
            this.graphView = graphView;

            RegisterCallback<MouseDownEvent>(OnMouseDown);
        }

        public void Initialize(HNGraphEdge edgeData, HNGraphBasePortView output, HNGraphBasePortView input)
        {
            this.edgeData = edgeData;
            this.output = output;
            ConnectOutput(OutputPortView);
            this.input = input;
            ConnectInput(InputPortView);
        }

        public void DisconnectOutput()
        {
            if(OutputPortView != null)
            {
                OutputPortView.DisconnectFromEdge(this);
                this.output = null;
            }
        }

        public void DisconnectInput()
        {
            if(InputPortView != null)
            {
                InputPortView.DisconnectFromEdge(this);
                this.input = null;
            }
        }

        public void DisconnectAll()
        {
            DisconnectOutput();
            DisconnectInput();
        }

        public HNGraphBasePortView GetAnotherPort(HNGraphBasePortView port)
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
        

        protected void ConnectOutput(HNGraphBasePortView outputPortView)
        {
            this.output = outputPortView;
            OutputPortView.ConnectToEdge(this);
        }

        protected void ConnectInput(HNGraphBasePortView inputPortView)
        {
            this.input = inputPortView;
            inputPortView.ConnectToEdge(this);
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
