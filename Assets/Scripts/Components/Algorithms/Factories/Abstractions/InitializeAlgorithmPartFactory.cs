using System;

using UnityEngine;

public class InitializeAlgorithmPartFactory<T> : MonoBehaviour
{
    #region Events

    public Action<T> OnVariableInitialized;

    #endregion
}
