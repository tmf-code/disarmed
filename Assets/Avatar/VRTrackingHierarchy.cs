using UnityEngine;
using System.Linq;

public class VRTrackingHierarchy : MonoBehaviour
{
  public Transform vrTrackingData;

  void Start()
  {
    var armature = transform.FindRecursiveOrThrow("Armature");
    vrTrackingData = transform.FindRecursiveOrThrow("VRTrackingData");
    var maybeCopyArmature = vrTrackingData.Find("Armature");
    var copyArmature = maybeCopyArmature != null ? maybeCopyArmature : Instantiate(armature, vrTrackingData);
    copyArmature.name = "Armature";

    // Get rid of any game components as we only need the transforms
    copyArmature.transform.TraverseChildren(child =>
    {
      var components = child.gameObject.GetComponents<Component>();
      components.ToList().Where(component => component.GetType() != typeof(Transform)).ToList().ForEach(component =>
      {
        Destroy(component);
      });
    });
  }
}

