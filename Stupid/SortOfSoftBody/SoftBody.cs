using UnityEngine;
using UnityEditor;

#region Custom Editor
#if UNITY_EDITOR

[CustomEditor(typeof(SoftBody)), CanEditMultipleObjects]
public class SoftBodyEditor : Editor
{
	SerializedProperty impactForceMultiplier;
	SerializedProperty impactForceOffset;
	SerializedProperty springForce;
	SerializedProperty damping;
	SerializedProperty minimumCollisionForce;
	SerializedProperty lastCollisionForce;
	SerializedProperty deformCollider;

	bool showDescription;
	bool showExperimental;

	void OnEnable()
	{
		impactForceMultiplier = serializedObject.FindProperty("impactForceMultiplier");
		impactForceOffset = serializedObject.FindProperty("impactForceOffset");
		springForce = serializedObject.FindProperty("springForce");
		damping = serializedObject.FindProperty("damping");
		deformCollider = serializedObject.FindProperty("deformCollider");
		minimumCollisionForce = serializedObject.FindProperty("minimumCollisionForce");
		lastCollisionForce = serializedObject.FindProperty("lastCollisionForce");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);

		Rect rect1 = EditorGUILayout.GetControlRect(false, 1);
		rect1.height = 1;
		EditorGUI.DrawRect(rect1, new Color(0.5f, 0.5f, 0.5f, 1));

		EditorGUILayout.Slider(impactForceMultiplier, -2, 2);

		EditorGUILayout.Slider(impactForceOffset, 0, 1);

		EditorGUILayout.Slider(springForce, 0, 100);

		EditorGUILayout.Slider(damping, 0, 100);

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Minimum Collision Force");
		minimumCollisionForce.floatValue = EditorGUILayout.FloatField(minimumCollisionForce.floatValue);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Last Collision Force");
		EditorGUILayout.LabelField(lastCollisionForce.floatValue.ToString());
		EditorGUILayout.EndHorizontal();

		Rect rect2 = EditorGUILayout.GetControlRect(false, 1);
		rect2.height = 1;
		EditorGUI.DrawRect(rect2, new Color(0.5f, 0.5f, 0.5f, 1));

		showExperimental = EditorGUILayout.Foldout(showExperimental, "Show Experimental Settings");

        if (showExperimental)
        {
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("Deform Collider");
			deformCollider.boolValue = EditorGUILayout.Toggle(deformCollider.boolValue);
			EditorGUILayout.EndHorizontal();
		}

		showDescription = EditorGUILayout.Foldout(showDescription, "Description");

		if (showDescription)
		{
			EditorGUILayout.HelpBox("This script deforms the mesh of the gameobject it is attached to on impact.", MessageType.Info);
		}

		serializedObject.ApplyModifiedProperties();
	}
}

#endif
#endregion

[RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshCollider)), AddComponentMenu("Physics/SoftBody")]
public class SoftBody : MonoBehaviour {

	public float impactForceMultiplier = 0.1f;
	public float impactForceOffset = 0;
	public float springForce = 20f;
	public float damping = 1;
	public bool deformCollider;

	public float minimumCollisionForce = 10;
	public float lastCollisionForce;

	Mesh filterMesh;
	Vector3[] filterOriginalVertices, filterDisplacedVertices;
	Vector3[] filterVertexVelocities;

	MeshCollider colliderComponent;
	Mesh colliderMesh;
	Vector3[] colliderOriginalVertices, colliderDisplacedVertices;
	Vector3[] colliderVertexVelocities;

	float uniformScale = 1f;

	void Start () {
		filterMesh = GetComponent<MeshFilter>().mesh;
		filterOriginalVertices = filterMesh.vertices;
		filterDisplacedVertices = new Vector3[filterOriginalVertices.Length];
		for (int i = 0; i < filterOriginalVertices.Length; i++) {
			filterDisplacedVertices[i] = filterOriginalVertices[i];
		}
		filterVertexVelocities = new Vector3[filterOriginalVertices.Length];

        if (deformCollider)
        {
			colliderComponent = GetComponent<MeshCollider>();
			colliderComponent.sharedMesh = (Mesh)Instantiate(colliderComponent.sharedMesh);
			colliderMesh = colliderComponent.sharedMesh;
			colliderOriginalVertices = colliderMesh.vertices;
			colliderDisplacedVertices = new Vector3[colliderOriginalVertices.Length];
			for (int i = 0; i < colliderOriginalVertices.Length; i++)
			{
				colliderDisplacedVertices[i] = colliderOriginalVertices[i];
			}
			colliderVertexVelocities = new Vector3[colliderOriginalVertices.Length];
		}
	}

	void Update () {
		uniformScale = transform.localScale.x;
		for (int i = 0; i < filterDisplacedVertices.Length; i++) {
			UpdateFilterVertex(i);
		}
		filterMesh.vertices = filterDisplacedVertices;
		filterMesh.RecalculateNormals();

		if (deformCollider)
		{
			if(colliderMesh == null)
            {
				colliderComponent = GetComponent<MeshCollider>();
				colliderComponent.sharedMesh = (Mesh)Instantiate(colliderComponent.sharedMesh);
				colliderMesh = colliderComponent.sharedMesh;
				colliderOriginalVertices = colliderMesh.vertices;
				colliderDisplacedVertices = new Vector3[colliderOriginalVertices.Length];
				for (int i = 0; i < colliderOriginalVertices.Length; i++)
				{
					colliderDisplacedVertices[i] = colliderOriginalVertices[i];
				}
				colliderVertexVelocities = new Vector3[colliderOriginalVertices.Length];
			}

			uniformScale = transform.localScale.x;
			for (int i = 0; i < colliderDisplacedVertices.Length; i++)
			{
				UpdateColliderVertex(i);
			}
			colliderMesh.vertices = colliderDisplacedVertices;
			colliderMesh.RecalculateNormals();

			colliderComponent.sharedMesh = colliderMesh;
		}
	}

	void UpdateFilterVertex (int i) {
		Vector3 velocity = filterVertexVelocities[i];
		Vector3 displacement = filterDisplacedVertices[i] - filterOriginalVertices[i];
		displacement *= uniformScale;
		velocity -= displacement * springForce * Time.deltaTime;
		velocity *= 1f - damping * Time.deltaTime;
		filterVertexVelocities[i] = velocity;
		filterDisplacedVertices[i] += velocity * (Time.deltaTime / uniformScale);
	}

	void UpdateColliderVertex(int i)
	{
		Vector3 velocity = colliderVertexVelocities[i];
		Vector3 displacement = colliderDisplacedVertices[i] - colliderOriginalVertices[i];
		displacement *= uniformScale;
		velocity -= displacement * springForce * Time.deltaTime;
		velocity *= 1f - damping * Time.deltaTime;
		colliderVertexVelocities[i] = velocity;
		colliderDisplacedVertices[i] += velocity * (Time.deltaTime / uniformScale);
	}

	public void AddDeformingForce (Vector3 point, float force) {
		point = transform.InverseTransformPoint(point);
		for (int i = 0; i < filterDisplacedVertices.Length; i++) {
			AddForceToFilterVertex(i, point, force);

            if (deformCollider)
            {
				AddForceToColliderVertex(i, point, force);
			}
		}
	}

	void AddForceToFilterVertex (int i, Vector3 point, float force) {
		Vector3 pointToVertex = filterDisplacedVertices[i] - point;
		pointToVertex *= uniformScale;
		float attenuatedForce = force / (1f + pointToVertex.sqrMagnitude);
		float velocity = attenuatedForce * Time.deltaTime;
		filterVertexVelocities[i] += pointToVertex.normalized * velocity;
	}

	void AddForceToColliderVertex(int i, Vector3 point, float force)
	{
		Vector3 pointToVertex = colliderDisplacedVertices[i] - point;
		pointToVertex *= uniformScale;
		float attenuatedForce = force / (1f + pointToVertex.sqrMagnitude);
		float velocity = attenuatedForce * Time.deltaTime;
		colliderVertexVelocities[i] += pointToVertex.normalized * velocity;
	}

	private void OnCollisionEnter(Collision collision)
    {
		float collisionForce = collision.impulse.magnitude / Time.fixedDeltaTime;

		lastCollisionForce = collisionForce;

		if(collisionForce > minimumCollisionForce)
        {
			foreach (ContactPoint cp in collision.contacts)
			{
				Vector3 point = cp.point;
				point += cp.normal * impactForceOffset;
				AddDeformingForce(point, -impactForceMultiplier * collisionForce / collision.contactCount);
			}
		}
    }
}