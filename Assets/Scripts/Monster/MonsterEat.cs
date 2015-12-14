﻿using UnityEngine;
using System.Collections;

public class MonsterEat : MonsterBase {

  public float totalGrowth = 2f;
  public float duration = 3f;

  private bool reachedFood = false;
  private bool eating = false;

  private MonsterCtrl ctrl;

  //===================================================================================================================

  protected override void Start() {
    //Get monster controller.
    ctrl = GetComponent<MonsterCtrl>();

    base.Start();
  }

  //===================================================================================================================

  private void OnEnable() {
    // target = findNearestFood();
  }

  //===================================================================================================================

  protected override void FixedUpdate() {
    
    reachedFood = isFoodClose();
    if(target == null) target = findNearestFood();
    if(!eating && reachedFood) StartCoroutine("eat");

    base.FixedUpdate();
  }

  //===================================================================================================================

  private void OnDisable() { StopCoroutine("eat"); }

  //===================================================================================================================

  private IEnumerator eat() {
    eating = true;
    reachedFood = false;
    anim.SetTrigger("StartEating");

    //Eat.
    float elapsedTime = 0;
    float elapsedGrowth = 0;
    Vector3 targetOriginalSize = target.localScale;
    while (elapsedTime < duration){
      //Shrink for food.
      target.localScale = targetOriginalSize * Mathf.Lerp(1, 0, elapsedTime/duration);
      //Grow for monster.
      float currentGrowth = Mathf.Lerp(0, totalGrowth, elapsedTime/duration);
      float growthStep = currentGrowth - elapsedGrowth;
      ctrl.grow(growthStep);
      elapsedGrowth = currentGrowth;
      elapsedTime += Time.deltaTime;
      yield return null;
    }
    ctrl.grow(totalGrowth - elapsedGrowth);

    anim.SetTrigger("StopEating");
    if(target != null) Destroy(target.gameObject);
    eating = false;
  }

  //===================================================================================================================

  private Transform findNearestFood() {
    
    Transform nearestFood = null;
    float nearestDistance = Mathf.Infinity;

    //Get all enemies, find the closest one.
    GameObject[] foods = GameObject.FindGameObjectsWithTag("Food");
    foreach(GameObject food in foods){
      float distance = Mathf.Abs(food.transform.position.x - transform.position.x);
      if(distance < nearestDistance){
        nearestDistance = distance;
        nearestFood = food.transform;
      }
    }
    return nearestFood;
  }

  //===================================================================================================================

  private bool isFoodClose() {
    if(target == null) return false;

    float distance = target.position.x - transform.position.x;
    return (Mathf.Abs(distance) < distanceThreshold);
  }

}
