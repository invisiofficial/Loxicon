using UnityEngine;

public class ReleaseAlgorithmPartFactory<T> : MonoBehaviour, ISerializable, IInitializable
{
    protected T _initializedName;
    
    private GameObject _initializePartObject;
    private int _initializePartIndex = -1;
    
    #region ISerializable implementation
    
    [System.Serializable]
    private class Serialization
    {
        public int InitializePartIndex;
    }

    public string Serialize()
    {
        Serialization data = new()
        {
            InitializePartIndex = _initializePartObject.transform.GetSiblingIndex()
        };

        return JsonUtility.ToJson(data);
    }
    
    public void Deserialize(string serializedData)
    {
        Serialization data = JsonUtility.FromJson<Serialization>(serializedData);
        
        _initializePartIndex = data.InitializePartIndex;
    }
    
    #endregion
    
    #region IInitializable implementation
    
    public void Initialize(params object[] args)
    {
        _initializePartIndex = _initializePartIndex == -1 ? args.Length - 2 : _initializePartIndex;
        _initializePartObject = args[_initializePartIndex] as GameObject;
        
        // Configuring the view
        Draggable draggable1 = _initializePartObject.GetComponent<Draggable>();
        Draggable draggable2 = this.GetComponent<Draggable>();

        draggable1.OnDeleted += () => Destroy(this.gameObject);
        draggable2.OnDeleted += () => Destroy(_initializePartObject);

        draggable1.SetLimits(draggable2, null);
        draggable2.SetLimits(null, draggable1);
        
        // Configuring the model   
        _initializePartObject.GetComponent<InitializeAlgorithmPartFactory<T>>().OnVariableInitialized += (T initializedName) => this._initializedName = initializedName;
    }
    
    #endregion
}
