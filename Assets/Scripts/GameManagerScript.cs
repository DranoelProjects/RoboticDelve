using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    [SerializeField] GameObject robotPrefab;
    [SerializeField] GameObject scientist;
    public int RobotNumber = 2;
    public bool LastRobotDead = true;
    public bool IsVirtualCamFollowingScientist = true;
    bool swapOnCd = false;
    float swapCd = 2f;
    CinemachineVirtualCamera virtualCamera;


    private void Awake()
    {
        virtualCamera = GameObject.Find("CMVcamScientist").GetComponent<CinemachineVirtualCamera>();
        scientist.GetComponent<PlayerScript>().CanMoove = true;
        Application.targetFrameRate = 60;
    }

    public void SwapBetweenRobotAndPlayer()
    {
        if (!swapOnCd && (RobotNumber > 0 || !IsVirtualCamFollowingScientist))
        {
            if (LastRobotDead)
            {
                RobotNumber--;
                PlayerPrefs.SetInt("RobotsBuilded", PlayerPrefs.GetInt("RobotsBuilded") + 1);
                LastRobotDead = false;
                robotPrefab.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                Instantiate(robotPrefab, new Vector3(scientist.transform.localPosition.x, scientist.transform.localPosition.y, 0), Quaternion.identity);
            }
            GameObject robot = GameObject.FindGameObjectWithTag("Robot");
            if (IsVirtualCamFollowingScientist)
            {
                virtualCamera.Follow = robot.transform;
                robot.GetComponent<PlayerScript>().CanMoove = true;
                scientist.GetComponent<PlayerScript>().CanMoove = false;
                IsVirtualCamFollowingScientist = false;

                scientist.transform.Find("Healthbar").gameObject.SetActive(false);
                robot.transform.Find("Healthbar").gameObject.SetActive(true);

            }
            else
            {
                virtualCamera.Follow = scientist.transform;
                robot.GetComponent<PlayerScript>().CanMoove = false;
                scientist.GetComponent<PlayerScript>().CanMoove = true;
                IsVirtualCamFollowingScientist = true;

                scientist.transform.Find("Healthbar").gameObject.SetActive(true);
                robot.transform.Find("Healthbar").gameObject.SetActive(false);
            }
            GameObject.Find("MainCanvas").GetComponent<UIScript>().StartSwapCouldown(swapCd);
            StartCoroutine(SwapCooldown());
        }
    }

    IEnumerator SwapCooldown()
    {
        swapOnCd = true;
        yield return new WaitForSeconds(swapCd);
        swapOnCd = false;
    }
}
