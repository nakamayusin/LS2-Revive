#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;


public class CharAnimateMaker : MonoBehaviour
{
    int animateCounter = 0;//0,1,2,for AniLength -----
    int keyFrameCounter = 0; //8,2,6 ------
    int dirCounter = 0;//16 ---------------
    int charCounter = 0;//10 -------------
    int colorCounter = 0;//4 -----------

    int sheetIndex = 0; // 1280 ------------

    int nowTitleNumber = 0; //------------
    int frameSpeed = 18;

    readonly string adrnPath = @"D:\download\LS2Reader_20190607\adrn.bin";

    List<string> NameList = new List<string>() {"Prince", "Bar", "Priest", "Old", "Cri", "Ninja", "Kid", "Knight", "Guard", "Explorer" };
    List<string> DirList = new List<string>() { "S", "SW", "W", "NW", "N", "NE", "E", "SE" };
    List<string> AnimateList = new List<string>() { "Atk", "Idle", "Walk" };
    List<string> WeaponList = new List<string>() { "Axe", "Bow", "Hand", "Sword", "Staff", "Spear" };
    List<int> AniLength = new List<int> { 8, 2, 6 };

    EditorCurveBinding spriteBinding = new EditorCurveBinding
    {
        type = typeof(SpriteRenderer),
        path = "",
        propertyName = "m_Sprite"
    };

    public static readonly int invo = 46084;

    public static readonly int weaponCounter = 5;//6種，每次運算手動切換 ----------

    public void Export()
    {
        
        animateCounter = keyFrameCounter = dirCounter = charCounter = colorCounter = sheetIndex = 0;

        using (StreamReader sr = new StreamReader(adrnPath))
        {
            Adrn.Init(sr.BaseStream);
        }
        Real real;

        var path = "Characters/" + NameList[charCounter];
        nowTitleNumber = invo;

        Sprite[] spr = Resources.LoadAll<Sprite>(nowTitleNumber.ToString());

        AnimatorController controller = Resources.Load<AnimatorController>(path + "/" + nowTitleNumber);

        AnimationClip clip = new AnimationClip();
        ObjectReferenceKeyframe[] spriteKeyFrames = new ObjectReferenceKeyframe[8];

        foreach (var img in Adrn.Images)
        {
            real = Adrn.Reals[img.Value];
            if (invo + 1280 == img.Key || invo + 1280*2 == img.Key || invo + 1280*3 == img.Key)
            {//每1280張換一個SpriteSheet檔案
                nowTitleNumber = img.Key;
                spr = Resources.LoadAll<Sprite>(nowTitleNumber.ToString());
                colorCounter++;//每1280張換一個角色
                sheetIndex = 0;
                charCounter = 0;
            }

            if (sheetIndex % 128 == 0)
            {//每128張換一個角色的Controller
                
                path = "Assets/Characters/" + NameList[charCounter];
                path += "/" + WeaponList[weaponCounter];
                path += "/" + colorCounter + "/";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                AssetDatabase.CopyAsset("Assets/Sample/SampleController.controller",
                    "Assets/Resources/Character/" + img.Key + ".controller");

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                controller = Resources.Load<AnimatorController>("Character/" + img.Key);

                charCounter++;
                
            }

            //每完成8或2或6幀後，重建Clip
            if (keyFrameCounter == 0)
            {
                if (animateCounter == 0)
                    frameSpeed = 18;
                else if(animateCounter == 1)
                    frameSpeed = 1;
                else if (animateCounter == 2)
                    frameSpeed = 12;

                clip = new AnimationClip
                {
                    frameRate = frameSpeed,   // FPS
                    legacy = false,
                };

                AnimationClipSettings animationClipSettings = new AnimationClipSettings
                {
                    loopTime = animateCounter != 0,
                };

                AnimationUtility.SetAnimationClipSettings(clip, animationClipSettings);

                //新創關鍵幀並貼上此圖
                spriteKeyFrames = new ObjectReferenceKeyframe[AniLength[animateCounter]];
                spriteKeyFrames[keyFrameCounter] = new ObjectReferenceKeyframe
                {
                    time =  1f / (float)AniLength[animateCounter] * (float)keyFrameCounter,
                    value = spr[sheetIndex],
                };
                AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, spriteKeyFrames);
                keyFrameCounter++;
            }
            else
            {
                //貼圖到下一幀
                spriteKeyFrames[keyFrameCounter] = new ObjectReferenceKeyframe
                {
                    time = 1f / (float)AniLength[animateCounter] * (float)keyFrameCounter,
                    value = spr[sheetIndex],
                };
                AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, spriteKeyFrames);
                keyFrameCounter++;
            }
            

            //這一套動畫完成後，前進下一個，每一組動作一個CLIP
            if (keyFrameCounter >= AniLength[animateCounter])
            {
                keyFrameCounter = 0;

                var layer = controller.layers[0].stateMachine;
                for (int i = 0; i < layer.states.Length; i++)
                {
                    if (layer.states[i].state.name == AnimateList[animateCounter] + " " + DirList[dirCounter])
                    {
                        layer.states[i].state.motion = clip;
                    }
                }

                //這裡clip要先存檔
                AssetDatabase.CreateAsset(clip, path + AnimateList[animateCounter] + " " + DirList[dirCounter] + ".anim");

                animateCounter++;

                if (animateCounter >= 3)
                {
                    animateCounter = 0;

                    dirCounter++;
                    if (dirCounter >= 8)
                        dirCounter = 0;
                }
                    
            }

            sheetIndex++;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }


    public void CalcPivot()
    {
        using (StreamReader sr = new StreamReader(adrnPath))
        {
            Adrn.Init(sr.BaseStream);
        }
        nowTitleNumber = invo;
        Real real;
        List<int> RepairedPivot = new List<int>();
        string objPath = "Assets/Resources/";
        TextureImporter nowpng;
        nowpng = AssetImporter.GetAtPath(objPath + nowTitleNumber + ".png") as TextureImporter;
        sheetIndex = 0;
        SpriteMetaData[] sliceMetaData = nowpng.spritesheet;
        foreach (var img in Adrn.Images)
        {
            if (invo + 1280 == img.Key || invo + 1280 * 2 == img.Key || invo + 1280 * 3 == img.Key)
            {//每1280張換一個SpriteSheet檔案
                nowpng.spritesheet = sliceMetaData;
                EditorUtility.SetDirty(nowpng);
                AssetDatabase.WriteImportSettingsIfDirty(objPath + nowTitleNumber + ".png");
                AssetDatabase.ImportAsset(objPath + nowTitleNumber + ".png", ImportAssetOptions.ForceUpdate);

                nowTitleNumber = img.Key;
                nowpng = AssetImporter.GetAtPath(objPath + nowTitleNumber + ".png") as TextureImporter;
                sliceMetaData = nowpng.spritesheet;

                sheetIndex = 0;
            }

            real = Adrn.Reals[img.Value];
            if (RepairedPivot.Contains(img.Value))
            {
                continue;
            }

            RepairedPivot.Add(img.Key);
            var a = (float)real.offsetX / (float)real.height * -1f;
            var b = ((float)real.width + (float)real.offsetY) / (float)real.width;
            if (real.offsetX == 0)
                a = 0.5f;

            if (real.offsetY == 0)
                b = 0.5f;

            sliceMetaData[sheetIndex].pivot = new Vector2(a, b);
            sliceMetaData[sheetIndex].alignment = 9;

            if (img.Key == invo + 1280 * 4 -1)
            {
                nowpng.spritesheet = sliceMetaData;
                EditorUtility.SetDirty(nowpng);
                AssetDatabase.WriteImportSettingsIfDirty(objPath + nowTitleNumber + ".png");
                AssetDatabase.ImportAsset(objPath + nowTitleNumber + ".png", ImportAssetOptions.ForceUpdate);
            }

            sheetIndex++;
        }
    }
}
#endif