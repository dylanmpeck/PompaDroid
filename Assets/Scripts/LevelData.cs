/*
 * Copyright (c) 2018 Razeware LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * Notwithstanding the foregoing, you may not use, copy, modify, merge, publish, 
 * distribute, sublicense, create a derivative work, and/or sell copies of the 
 * Software in any work that is designed, intended, or marketed for pedagogical or 
 * instructional purposes related to programming, coding, application development, 
 * or information technology.  Permission for such use, copying, modification,
 * merger, publication, distribution, sublicensing, creation of derivative works, 
 * or sale is expressly withheld.
 *    
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using UnityEngine;
using System;
using System.Collections.Generic;

//The LevelData class has the CreateAssetMenu attribute, which allows you to create a LevelData ScriptableObject from within the Assets / Create submenu found in Unity’s menu bar.
[CreateAssetMenu(fileName ="LevelData", menuName = "Pompadroid/LevelData")]
public class LevelData : ScriptableObject {
	public List<BattleEvent> battleData;
	public GameObject levelPrefab;
	public string levelName;
}

//Serializable attribute means the contents of the class display in the Inspector.
//the battle event, which will happen when the position.x of the gamemanager reaches column value
//enemies parameter defines all the enemies that will spawn
[Serializable]
public class BattleEvent {
	public int column;
	public List<EnemyData> enemies;
}

//the enemy data containing the type of enemy, 
//the color of the enemy (if robot), 
//the row which it will spawn, 
//and the offset from the center 
[Serializable]
public class EnemyData {
	public EnemyType type;
	public RobotColor color;
	public int row;
	public float offset;
}

//the enumeration type to state whether the enemy is a robot or a boss
public enum EnemyType {
	Robot = 0,
	Boss
}