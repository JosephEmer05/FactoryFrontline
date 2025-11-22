using UnityEngine;
using System.Collections.Generic;

public class PackageConveyorManager : MonoBehaviour
{
    [Header("Conveyor Config")]
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    //[SerializeField] private Transform packageHolder;

    [Header("Package Config")]
    [SerializeField] private Vector3 itemSpacing;
    [SerializeField] private float itemSpeed;

    private List<PackageInstance> packages = new List<PackageInstance>();

    void Update()
    {
        UpdatePackagePositions();
    }

    public void AddPackage(PackageInstance package)
    {
        /*if (!packages.Contains(package))
        {
            packages.Add(package);
        }*/

        packages.Add(package);

        SortByPosition();
    }

    public void RemovePackage(PackageInstance package)
    {
        packages.Remove(package);

        SortByPosition();
        UpdatePackagePositions();
    }

    void SortByPosition()
    {
        packages.Sort((a, b) =>
        {
            if (a.isDragging && b.isDragging) return 0;
            if (a.isDragging) return 0;
            if (b.isDragging) return 0;

            float aDist = Vector3.Distance(a.transform.position, endPoint.position);
            float bDist = Vector3.Distance(b.transform.position, endPoint.position);
            return aDist.CompareTo(bDist);
        });
    }

    void UpdatePackagePositions()
    {
        Vector3 targetPos = endPoint.position;

        for (int i = 0; i < packages.Count; i++)
        {
            PackageInstance package = packages[i];

            if (package.isDragging)
            {
                targetPos -= new Vector3(itemSpacing.x, itemSpacing.y, itemSpacing.z);
                continue;
            }

            package.transform.position = Vector3.MoveTowards(package.transform.position, targetPos, itemSpeed * Time.deltaTime);
            package.originalPos = targetPos;

            targetPos -= new Vector3(itemSpacing.x, itemSpacing.y, itemSpacing.z);
        }
    }
}
