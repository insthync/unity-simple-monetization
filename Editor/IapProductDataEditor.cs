using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Purchasing;
using UnityEngine;
using UnityEngine.Purchasing;

[CustomEditor(typeof(IapProductData))]
[CanEditMultipleObjects]
public class IapProductDataEditor : Editor
{
    private const string kNoProduct = "<None>";

    private List<string> m_ValidIDs = new List<string>();
    private SerializedProperty m_ProductIDProperty;

    public void OnEnable()
    {
        m_ProductIDProperty = serializedObject.FindProperty("productId");
    }

    public override void OnInspectorGUI()
    {
        IapProductData productData = (IapProductData)target;

        serializedObject.Update();

        EditorGUILayout.LabelField(new GUIContent("Product ID:", "Select a product from the IAP catalog"));

        var catalog = ProductCatalog.LoadDefaultCatalog();

        m_ValidIDs.Clear();
        m_ValidIDs.Add(kNoProduct);
        foreach (var product in catalog.allProducts)
        {
            m_ValidIDs.Add(product.id);
        }

        int currentIndex = string.IsNullOrEmpty(productData.productId) ? 0 : m_ValidIDs.IndexOf(productData.productId);
        int newIndex = EditorGUILayout.Popup(currentIndex, m_ValidIDs.ToArray());
        if (newIndex > 0 && newIndex < m_ValidIDs.Count)
        {
            m_ProductIDProperty.stringValue = m_ValidIDs[newIndex];
        }
        else
        {
            m_ProductIDProperty.stringValue = string.Empty;
        }

        if (GUILayout.Button("IAP Catalog..."))
        {
            ProductCatalogEditor.ShowWindow();
        }

        DrawPropertiesExcluding(serializedObject, new string[] { "m_Script" });

        serializedObject.ApplyModifiedProperties();
    }
}
