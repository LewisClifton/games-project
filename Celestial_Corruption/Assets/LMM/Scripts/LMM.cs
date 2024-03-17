using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.Barracuda;

public class LMM
{

    private Model projectorModel;
    private Model decompressorModel;
    private Model stepperModel;

    private IWorker projectorWorker;
    private IWorker decompressorWorker;
    private IWorker stepperWorker;

    List<float> projectorInputMean = new List<float>{};
    List<float> projectorInputStd = new List<float>{};
    List<float> projectorOutputMean = new List<float>{};
    List<float> projectorOutputStd = new List<float>{};
    List<float> stepperInputMean = new List<float>{};

    List<float> stepperInputStd = new List<float>{};
    List<float> stepperOutputMean = new List<float>{};
    List<float> stepperOutputStd = new List<float>{};
    List<float> decompressorInputMean = new List<float>{};
    List<float> decompressorInputStd = new List<float>{};

    List<float> decompressorOutputMean = new List<float>{};
    List<float> decompressorOutputStd = new List<float>{};

    public void LoadModels(NNModel projectorModelAsset, NNModel decompressorModelAsset, NNModel stepperModelAsset) 
    {   
        projectorModel = ModelLoader.Load(projectorModelAsset, true, false);
        decompressorModel = ModelLoader.Load(decompressorModelAsset, false, false);
	    stepperModel = ModelLoader.Load(stepperModelAsset, false, false);

        projectorWorker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, projectorModel);
        decompressorWorker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, decompressorModel);
	    stepperWorker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, stepperModel);
    }

    public void LoadNormalData(TextAsset projectorInputMeanPath,
                            TextAsset projectorInputStdPath,
                            TextAsset projectorOutputMeanPath,
                            TextAsset projectorOutputStdPath,
                            TextAsset stepperInputMeanPath,
                            TextAsset stepperInputStdPath,
                            TextAsset stepperOutputMeanPath,
                            TextAsset stepperOutputStdPath,
                            TextAsset decompressorInputMeanPath,
                            TextAsset decompressorInputStdPath,
                            TextAsset decompressorOutputMeanPath,
                            TextAsset decompressorOutputStdPath)
    {
        LoadDataFile(projectorInputMeanPath, projectorInputMean);
        LoadDataFile(projectorInputStdPath, projectorInputStd);
        LoadDataFile(projectorOutputMeanPath, projectorOutputMean);
        LoadDataFile(projectorOutputStdPath, projectorOutputStd);
        LoadDataFile(stepperInputMeanPath, stepperInputMean);
        LoadDataFile(stepperInputStdPath, stepperInputStd);
        LoadDataFile(stepperOutputMeanPath, stepperOutputMean);
        LoadDataFile(stepperOutputStdPath, stepperOutputStd);
        LoadDataFile(decompressorInputMeanPath, decompressorInputMean);
        LoadDataFile(decompressorInputStdPath, decompressorInputStd);
        LoadDataFile(decompressorOutputMeanPath, decompressorOutputMean);
        LoadDataFile(decompressorOutputStdPath, decompressorOutputStd);
    }

    public void LoadDataFile(TextAsset file, List<float> data)
    {
        // Read the text files which contain the data for normalising the data

        try
        {
            string[] lines = file.text.Split('\n');

            foreach (string line in lines)
            {
                if (float.TryParse(line, out float floatValue))
                {
                    data.Add(floatValue);
                }
               
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error loading TextAsset: " + e.Message);
        }
    }

    public void ExecuteProjector(Tensor projectorInput, ref Tensor projectorOutput)
    {
        // Normalise input in-place
        for (int i = 0; i < projectorInput.length; i++)
        {
            projectorInput[0, 0, i, 0] = (projectorInput[0, 0, i, 0] - projectorInputMean[i]) / (projectorInputStd[i]);
        }
        // Does above cause issues?

        // Run the projector
        projectorWorker.Execute(projectorInput);
        projectorOutput = projectorWorker.CopyOutput("output").DeepCopy();

        // De-normalise output in-place
        for (int i = 0; i < projectorOutput.length; i++)
        {
            projectorOutput[0, 0, i, 0] = projectorOutput[0, 0, i, 0] * projectorOutputStd[i] + projectorOutputMean[i];
        }

        // Worker input has to be passed by value so dispose inside this function
        projectorInput.Dispose();
    }


    public void ExecuteStepper(Tensor stepperInput, ref Tensor stepperOutput, float dt)
    {
        // Normalise input in-place
        for (int i = 0; i < stepperInput.length; i++)
        {
            stepperInput[0, 0, i, 0] =  (stepperInput[i] - stepperInputMean[i]) / (stepperInputStd[i]+1f);
        }

        // Run the stepper
        stepperWorker.Execute(stepperInput);
        stepperOutput = stepperWorker.CopyOutput("output").DeepCopy();

        // De-Normalise output in-place
        for (int i = 0; i < stepperOutput.length; i++)
        {
            stepperInput[0, 0, i, 0] += ((stepperOutput[0, 0, i, 0] * stepperOutputStd[i]) + stepperOutputMean[i]) * dt;
        }

        stepperOutput = stepperInput.DeepCopy();

        // Worker input has to be passed by value so dispose inside this function
        stepperInput.Dispose();
    }


    public void ExecuteDecompressor(Tensor decompressorInput, ref Tensor decompressorOutput)
    {   
        for (int i = 0; i < decompressorInput.length; i++)
			decompressorInput[0, 0, i, 0] = (decompressorInput[i] - decompressorInputMean[i]) / decompressorInputStd[i];
        // Does above cause issues?

        // Run the decompressor
        decompressorWorker.Execute(decompressorInput);
        decompressorOutput = decompressorWorker.CopyOutput("output").DeepCopy();

        // De-normalise output in-place
        for (int i = 0; i < decompressorOutput.length; i++)
            decompressorOutput[0, 0, i, 0] = (decompressorOutput[0, 0, i, 0] * (decompressorOutputStd[i])) + decompressorOutputMean[i];

        // Worker input has to be passed by value so dispose inside this function
        decompressorInput.Dispose();
    }

    public void DisposeWorkers()
    {
        projectorWorker.Dispose();
        decompressorWorker.Dispose();
        stepperWorker.Dispose();
    }
}
