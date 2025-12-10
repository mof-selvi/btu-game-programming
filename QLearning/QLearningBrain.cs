using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class QLearningBrain : MonoBehaviour
{
    [Header("General Settings")]
    public float learningRate = 0.1f;
    public float discount = 0.95f;
    public float exploration = 0.2f;

    [Header("File Settings")]
    public string saveFileName = "RLBrain.json";

    // --- Input / Action Storage ---
    private List<float> currentInputs = new();     // dynamic input vector
    private List<ActionDefinition> actions = new(); // action registry

    // Q-Table : key = state string, value = Q-values for actions
    private Dictionary<string, float[]> Q = new();

    private string savePath;

    // ===================================================================================
    //  STRUCTS
    // ===================================================================================

    [Serializable]
    public class ActionDefinition
    {
        public string actionName;
        public Action<object[]> method;
        public int parameterCount;

        public ActionDefinition(string name, Action<object[]> func, int paramCount)
        {
            actionName = name;
            method = func;
            parameterCount = paramCount;
        }
    }

    [Serializable]
    private class SaveModel
    {
        public int inputCount;
        public int actionCount;
        public Dictionary<string, float[]> qTable;
    }

    // ===================================================================================
    // UNITY
    // ===================================================================================

    void Awake()
    {
        savePath = Path.Combine(Application.dataPath, saveFileName);
        LoadOrCreateModel();
    }

    // ===================================================================================
    // PUBLIC API USED BY OTHER SCRIPTS
    // ===================================================================================

    /// <summary>
    /// Register input values every frame.
    /// Called by enemy AI script before calling DecideAction().
    /// </summary>
    public void SetInputs(List<float> inputs)
    {
        currentInputs = inputs;
    }

    /// <summary>
    /// Register an available action the agent can take.
    /// </summary>
    public void RegisterAction(string name, Action<object[]> method, int parameterCount)
    {
        actions.Add(new ActionDefinition(name, method, parameterCount));
    }

    /// <summary>
    /// Call this every decision tick. It returns which action index to execute.
    /// </summary>
    public int DecideAction()
    {
        string state = EncodeState(currentInputs);
        EnsureStateExists(state);

        if (UnityEngine.Random.value < exploration)
        {
            return UnityEngine.Random.Range(0, actions.Count);
        }

        float[] qRow = Q[state];

        int bestIndex = 0;
        float bestVal = qRow[0];

        for (int i = 1; i < qRow.Length; i++)
        {
            if (qRow[i] > bestVal)
            {
                bestVal = qRow[i];
                bestIndex = i;
            }
        }

        return bestIndex;
    }

    /// <summary>
    /// Execute the selected action.
    /// </summary>
    public void ExecuteAction(int actionIndex, params object[] parameters)
    {
        actions[actionIndex].method.Invoke(parameters);
    }

    /// <summary>
    /// Give a positive reward.
    /// </summary>
    public void Reward(float value)
    {
        ApplyReward(value);
    }

    /// <summary>
    /// Give a negative reward.
    /// </summary>
    public void Punish(float value)
    {
        ApplyReward(-Mathf.Abs(value));
    }

    // ===================================================================================
    // INTERNAL RL LOGIC
    // ===================================================================================

    private void ApplyReward(float reward)
    {
        string oldState = EncodeState(currentInputs);
        EnsureStateExists(oldState);

        int action = DecideAction(); // the action the agent took

        float[] qRow = Q[oldState];

        // After reward, compute next state
        string newState = EncodeState(currentInputs);
        EnsureStateExists(newState);

        float maxNext = Max(Q[newState]);

        qRow[action] = qRow[action] + learningRate * (reward + discount * maxNext - qRow[action]);

        SaveModelToFile();
    }

    private float Max(float[] arr)
    {
        float m = arr[0];
        for (int i = 1; i < arr.Length; i++)
            if (arr[i] > m) m = arr[i];
        return m;
    }

    private void EnsureStateExists(string state)
    {
        if (!Q.ContainsKey(state))
        {
            Q[state] = new float[actions.Count];
        }
    }

    // ===================================================================================
    // SERIALIZATION
    // ===================================================================================

    private string EncodeState(List<float> inputs)
    {
        return string.Join("_", inputs);
    }

    private void LoadOrCreateModel()
    {
        if (!File.Exists(savePath))
        {
            Debug.Log("RLBrain: Creating new model file.");
            CreateNewModel();
            return;
        }

        string json = File.ReadAllText(savePath);
        var model = JsonUtility.FromJson<SaveModel>(json);

        // Compare structure
        if (model.inputCount != currentInputs.Count ||
            model.actionCount != actions.Count)
        {
            Debug.LogWarning("RLBrain: Structure changed. Creating new model.");
            CreateNewModel();
            return;
        }

        Q = model.qTable;
        Debug.Log("RLBrain: Model loaded.");
    }

    private void CreateNewModel()
    {
        Q = new Dictionary<string, float[]>();
        SaveModelToFile();
    }

    private void SaveModelToFile()
    {
        var model = new SaveModel
        {
            inputCount = currentInputs.Count,
            actionCount = actions.Count,
            qTable = Q
        };

        string json = JsonUtility.ToJson(model, true);
        File.WriteAllText(savePath, json);
    }
}
