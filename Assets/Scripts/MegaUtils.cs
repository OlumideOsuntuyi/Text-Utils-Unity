using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;

using LostOasis;

using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;

using Object = UnityEngine.Object;

public static class MegaUtils
{
    public static System.Random Random
    {
        get
        {
            if (rand == null)
                rand = new(DateTime.Now.Millisecond ^ DateTime.Now.Second ^ DateTime.Now.Hour);
            return rand;
        }
    }


    public static Vector3 RandomDirection()
    {
        return new Vector3(RandomValue(), RandomValue(), RandomValue());
    }

    public static float RandomValue()
    {
        return rand.Next(-1000, 1001) * 0.001f;
    }
    public static float RandomValue01()
    {
        return rand.Next(0, 1000001) * 0.000001f;
    }

    public static string ReduceNumberStringLength(float value)
    {
        float reduced_value = Mathf.FloorToInt(value);
        string text = reduced_value.ToString();

        if (value >= 1000000000)
        {
            reduced_value = Mathf.Round(value / 100000) / 100;
            text = reduced_value.ToString() + "b";
        }
        else if (value >= 1000000)
        {
            reduced_value = Mathf.Round(value / 10000);
            if (reduced_value > 100000)
            {
                reduced_value = Mathf.Round(reduced_value / 10) * 10;
            }
            else
            {

            }
            reduced_value /= 100;
            text = reduced_value.ToString() + "m";
        }
        else if (value >= 10000)
        {
            reduced_value = Mathf.Round(value / 100);
            if(reduced_value > 1000)
            {
                reduced_value = Mathf.Round(reduced_value / 10) * 10;
            }
            else
            {

            }
            reduced_value /= 10;
            text = reduced_value.ToString() + "k";
        }

        return text;
    }

    public static string AbbreviateNumber(float number)
    {
        if (number >= 1000000)
        {
            return (number / 1000000f).ToString("0.#") + "M";
        }
        else if (number >= 1000)
        {
            return (number / 1000f).ToString("0.#") + "k";
        }
        else
        {
            return number.ToString();
        }
    }

    public static string AddCommasToNumber(float number)
    {
        return ((int)number).ToString("N0");
    }

    public static string[] ConvertTo12HourTime(float second)
    {
        string[] time = new string[4];
        int n_seconds = Mathf.FloorToInt(second % 60);
        int n_minutes = Mathf.FloorToInt(second % 3600 / 60);
        int n_hours = Mathf.FloorToInt(second % 86400 / 3600);
        time[2] = n_seconds > 9 ? $"{n_seconds}" : $"0{n_seconds}";


        time[1] = n_minutes > 9 ? $"{n_minutes}" : $"0{n_minutes}";


        if (n_hours > 11)
        {
            n_hours -= 12;
            time[3] = "pm";
        }
        else
        {
            time[3] = "am";
        }


        time[0] = n_hours > 9 ? $"{n_hours}" : $"0{n_hours}";
        return time;
    }
    public static string ShortenString(string text, int length)
    {
        string shortened_text = text;
        if (shortened_text.Length > length + 2)
        {
            _ = shortened_text.Remove(length + 1);
        }
        return shortened_text;
    }
    public static string GetMaxTimeUnit(float seconds)
    {
        int totalSeconds = Mathf.RoundToInt(seconds);
        int days = totalSeconds / 86400;
        int hours = (totalSeconds % 86400) / 3600;
        int minutes = (totalSeconds % 3600) / 60;
        int remainingSeconds = totalSeconds % 60;

        if (days > 0)
        {
            return days.ToString() + "d";
        }
        else if (hours > 0)
        {
            return hours.ToString() + "h";
        }
        else if(minutes > 0)
        {
            return minutes.ToString() + "m";
        }
        else
        {
            return seconds.ToString() + "s";
        }

    }
    public static string ShortenTimeInSeconds(float seconds = 0)
    {
        int totalSeconds = Mathf.RoundToInt(seconds);
        int days = totalSeconds / 86400;
        int hours = (totalSeconds % 86400) / 3600;
        int minutes = (totalSeconds % 3600) / 60;
        int remainingSeconds = totalSeconds % 60;

        if (days > 0)
        {
            return string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D2}", days, hours, minutes, remainingSeconds);
        }
        else if (hours > 0)
        {
            return string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, remainingSeconds);
        }
        else
        {
            return string.Format("{0:D2}:{1:D2}", minutes, remainingSeconds);
        }
    }
    public static string AddZero(int number, int digits)
    {
        string numberStr = number.ToString();
        if (numberStr.Length < digits)
        {
            for (int i = 0; i < digits - numberStr.Length; i++)
            {
                numberStr = "0" + numberStr;
            }
            return numberStr;
        }
        else
        {
            return numberStr;
        }
    }
    public static string ToZeroString(int number, int zeros)
    {
        string value = "";
        for (int i = 0; i < zeros; i++)
        {
            if (number < Mathf.Pow(10, i - 1))
            {
                value += "0";
            };
        };
        return value + number.ToString();
    }
    public static string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        StringBuilder sb = new StringBuilder(length);

        for (int i = 0; i < length; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, chars.Length - 1);
            sb.Append(chars[randomIndex]);
        }

        return sb.ToString();
    }
    public static string GenerateRandomString(int seed, int length)
    {
        // Combine the seed with a unique identifier
        int combinedSeed = seed ^ Guid.NewGuid().GetHashCode();
        System.Random random = new(combinedSeed);

        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        StringBuilder result = new StringBuilder(length);
        for (int i = 0; i < length; i++)
        {
            result.Append(chars[random.Next(chars.Length)]);
        }
        return result.ToString();
    }

    public static string GenerateRandomLetters(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        StringBuilder sb = new StringBuilder(length);

        for (int i = 0; i < length; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, chars.Length - 1);
            sb.Append(chars[randomIndex]);
        }
        string text = sb.ToString();
        text.ToLower();
        //text.FirstCharacterToUpper();
        return text;
    }
    public static TMP_Text SetTextSize(TMP_Text text, float minSize, float maxSize, int minLength, int maxLength)
    {
        string str = text.text;
        if (str.Length > minLength)
        {
            float range = (str.Length - minLength) / (maxLength - minLength);
            text.fontSize = Mathf.Lerp(minSize, maxSize, range);
        }

        return text;
    }
    public static TMP_Text SetTextSizeWithRect(TMP_Text text, float minSize, float maxSize, float minLength)
    {
        float size = text.rectTransform.sizeDelta.x;
        int len = text.text.Length;
        if (len > minLength)
        {
            float fontSize = Mathf.Clamp(size / len, minSize, maxSize);
            text.fontSize = fontSize;
        }
        return text;
    }
    public static bool IsTouchAboveImage(Vector2 touchPosition, RectTransform imageRect)
    {
        Vector2 imageMin = imageRect.position;
        Vector2 imageMax = imageRect.position + new Vector3(imageRect.rect.width, imageRect.rect.height);

        if (
            touchPosition.x >= imageMin.x &&
            touchPosition.x <= imageMax.x &&
            touchPosition.y >= imageMin.y &&
            touchPosition.y <= imageMax.y
        )
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public static Vector3 ClampVector(Vector3 vector, float min, float max)
    {
        float x = Mathf.Clamp(vector.x, min, max);
        float y = Mathf.Clamp(vector.y, min, max);
        float z = Mathf.Clamp(vector.z, min, max);
        return new Vector3(x, y, z);
    }
    public static Vector3 GetScreenDirection(Vector3 point, Vector3 src)
    {
        float x = point.x - src.x;
        float y = point.y - src.y;
        float z = point.z - src.z;
        Vector3 vct = new Vector3(x, y, z);
        vct.x /= Screen.currentResolution.width;
        vct.y /= Screen.currentResolution.height;
        return vct;
    }
    public static float Distance(Vector3 vector1, Vector3 vector2)
    {
        float x = Mathf.Pow(vector1.x - vector2.x, 2);
        float y = Mathf.Pow(vector1.y - vector2.y, 2);
        float z = Mathf.Pow(vector1.z - vector2.z, 2);
        return Mathf.Sqrt(x + y + z);
    }
    public static float ScreenTouchAngle(Vector2 startValue, Vector2 endValue)
    {
        var center = new Vector2(Screen.width / 2, Screen.height / 2);
        var angleTouch = Vector2.Angle(startValue - center, endValue - center);
        return angleTouch;
    }
    public static Vector3 AddRelativeVelocity(Vector3 localVelocity, Vector3 relativeVector, float magnitude)
    {
        // Calculate the relative velocity vector
        Vector3 relativeVelocity = relativeVector.normalized * magnitude;

        // Add the relative velocity to the current velocity
        localVelocity += relativeVelocity;
        return localVelocity;
    }
    public static bool IsWithinRadius(Vector3 point, Vector3 centerPoint, float radius)
    {
        float distance = Vector3.Distance(point, centerPoint);
        return distance <= radius;
    }
    public static void ClampTransformToRadius(Transform transformToClamp, Vector3 centerPoint, float radius)
    {
        Vector3 displacement = transformToClamp.localPosition - centerPoint;
        if (displacement.magnitude > radius)
        {
            Vector3 clampedPosition = centerPoint + displacement.normalized * radius;
            transformToClamp.localPosition = clampedPosition;
        }
    }
    public static Vector3 GetRandomPointInCircle(Vector3 centerPoint, float minDistance, float maxDistance)
    {
        float randomRadius = UnityEngine.Random.Range(minDistance, maxDistance);
        float randomAngle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);

        float xOffset = randomRadius * Mathf.Cos(randomAngle);
        float zOffset = randomRadius * Mathf.Sin(randomAngle);

        Vector3 randomPoint = centerPoint + new Vector3(xOffset, 0f, zOffset);
        return randomPoint;
    }
    public static List<Vector3> GenerateBatchSpawnPoints(Vector3 objectSize, Vector3 firstObjectPosition, int minRows, int maxRows, int minColumns, int maxColumns, float spacing)
    {
        List<Vector3> spawnPoints = new List<Vector3>();

        int numRows = UnityEngine.Random.Range(minRows, maxRows + 1);
        int numColumns = UnityEngine.Random.Range(minColumns, maxColumns + 1);

        for (int row = 0; row < numRows; row++)
        {
            for (int column = 0; column < numColumns; column++)
            {
                float xOffset = column * (objectSize.x + spacing);
                float zOffset = row * (objectSize.z + spacing);

                Vector3 spawnPoint = firstObjectPosition + new Vector3(xOffset, 0f, zOffset);
                spawnPoints.Add(spawnPoint);
            }
        }

        return spawnPoints;
    }

    static System.Random rand;
    public static bool GetRandomChance(float percentage)
    {
        if (percentage <= 0f)
        {
            return false;
        }
        else if (percentage >= 100f)
        {
            return true;
        }
        else
        {
            percentage *= 100;
            float randomValue = Random.Next(0, 2) == 0 ? 
                Mathf.Max(Random.Next(0, 10000), Random.Next(0, 10000)) : 
                Mathf.Min(Random.Next(0, 10000), Random.Next(0, 10000));
            return randomValue <= percentage;
        }
    }

    public static void ClearChildren(Transform content)
    {
        foreach (Transform child in content)
        {
            if(Application.isPlaying)
            {
                Object.Destroy(child.gameObject);
            }
            else
            {
                Object.DestroyImmediate(child.gameObject);
            }
        }
    }

    public static string InsertColor(string text, Color color)
    {
        return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{text}</color>";
    }

    public static string ReplaceText(string value, string word, Color wordFoundColor)
    {
        if (value.Length <= 1) return value;

        string pattern = Regex.Escape(word);
        string replacement = InsertColor(word, wordFoundColor);

        return Regex.Replace(value, pattern, match => InsertColor(match.Value, wordFoundColor), RegexOptions.IgnoreCase);
    }
}
public static class RaycastUtilities
{
    public static bool PointerIsOverUI(Vector2 screenPos)
    {
        var hitObject = UIRaycast(ScreenPosToPointerData(screenPos));
        return hitObject != null && hitObject.layer == LayerMask.NameToLayer("UI");
    }
    public static GameObject PointerOverUI(Vector2 screenPos)
    {
        if (PointerIsOverUI(screenPos))
        {
            var hitObject = UIRaycast(ScreenPosToPointerData(screenPos));
            return hitObject;
        }
        return null;
    }
    static GameObject UIRaycast(PointerEventData pointerData)
    {
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        return results.Count < 1 ? null : results[0].gameObject;
    }

    public static PointerEventData ScreenPosToPointerData(Vector2 screenPos)
       => new(EventSystem.current) { position = screenPos };



}

namespace LostOasis
{
    public class EnumUtilities
    {
        /// <summary>
        /// Parses the string representation of an enum baseValue and returns the corresponding enum baseValue.
        /// </summary>
        /// <typeparam name="T">The enum type.</typeparam>
        /// <param name="name">The string representation of the enum baseValue.</param>
        /// <returns>The enum baseValue of type T.</returns>
        public static T GetEnum<T>(string name)
        {
            if (Enum.TryParse(typeof(T), name, out object enumValue))
            {
                return (T)enumValue;
            }
            throw new ArgumentException("Invalid enum name.");
        }

        /// <summary>
        /// Tries to parse the string representation of an enum baseValue and assigns the corresponding enum baseValue to the target out parameter.
        /// </summary>
        /// <typeparam name="T">The enum type.</typeparam>
        /// <param name="name">The string representation of the enum baseValue.</param>
        /// <param name="target">The variable to store the parsed enum baseValue.</param>
        /// <returns>True if the parsing was successful; otherwise, false.</returns>
        public static bool TryGetEnum<T>(string name, out T target)
        {
            bool found = Enum.TryParse(typeof(T), name, out object result);
            target = (T)result;
            return found;
        }
    }
    public static class ListUtilities
    {
        public static T GetRandomItemInList<T>(List<T> list)
        {
            return list[UnityEngine.Random.Range(0, list.Count)];
        }
        public static List<T> ShuffleAndLimit<T>(this List<T> list, int maxLength)
        {
            System.Random random = new System.Random();
            int n = list.Count;
            if (n <= maxLength)
            {
                // If the list length is less than or equal to the maxLength, return the list as is
                return list;
            }
            else
            {
                List<T> shuffledList = new List<T>(list);
                // Perform the Fisher-Yates shuffle
                for (int i = n - 1; i > 0; i--)
                {
                    int j = random.Next(i + 1);
                    T temp = shuffledList[i];
                    shuffledList[i] = shuffledList[j];
                    shuffledList[j] = temp;
                }
                // Return a sub-list with the first maxLength elements
                return shuffledList.GetRange(0, maxLength);
            }
        }
    }
    public static class DeepCopyHelper
    {
        public static T DeepCopy<T>(T obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj), "Object to be copied cannot be null.");

            // Perform the deep copy using serialization and deserialization.
            using MemoryStream memoryStream = new MemoryStream();
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(memoryStream, obj);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return (T)formatter.Deserialize(memoryStream);
        }
    }
}
