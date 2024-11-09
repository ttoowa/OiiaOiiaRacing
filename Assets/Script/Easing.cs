using UnityEngine;

public static class Easing {
    public static float EasePreset(float t, EasingPreset preset) {
        switch (preset) {
            case EasingPreset.Linear:
                return Linear(t);
            case EasingPreset.EaseInSine:
                return EaseInSine(t);
            case EasingPreset.EaseOutSine:
                return EaseOutSine(t);
            case EasingPreset.EaseInOutSine:
                return EaseInOutSine(t);
            case EasingPreset.EaseInCubic:
                return EaseInCubic(t);
            case EasingPreset.EaseOutCubic:
                return EaseOutCubic(t);
            case EasingPreset.EaseInOutCubic:
                return EaseInOutCubic(t);
            case EasingPreset.EaseInQuint:
                return EaseInQuint(t);
            case EasingPreset.EaseOutQuint:
                return EaseOutQuint(t);
            case EasingPreset.EaseInOutQuint:
                return EaseInOutQuint(t);
            case EasingPreset.EaseInElastic:
                return EaseInElastic(t);
            case EasingPreset.EaseOutElastic:
                return EaseOutElastic(t);
            case EasingPreset.EaseInOutElastic:
                return EaseInOutElastic(t);
            case EasingPreset.EaseInBack:
                return EaseInBack(t);
            case EasingPreset.EaseOutBack:
                return EaseOutBack(t);
            case EasingPreset.EaseInOutBack:
                return EaseInOutBack(t);
            case EasingPreset.EaseInBounce:
                return EaseInBounce(t);
            case EasingPreset.EaseOutBounce:
                return EaseOutBounce(t);
            case EasingPreset.EaseInOutBounce:
                return EaseInOutBounce(t);
            default:
                return t;
        }
    }

    public static float Linear(float t) {
        return t;
    }

    public static float EaseInSine(float t) {
        return 1 - Mathf.Cos(t * Mathf.PI / 2);
    }

    public static float EaseOutSine(float t) {
        return Mathf.Sin(t * Mathf.PI / 2);
    }

    public static float EaseInOutSine(float t) {
        return -(Mathf.Cos(Mathf.PI * t) - 1) / 2;
    }

    public static float EaseInCubic(float t) {
        return t * t * t;
    }

    public static float EaseOutCubic(float t) {
        return 1 - Mathf.Pow(1 - t, 3);
    }

    public static float EaseInOutCubic(float t) {
        return t < 0.5f ? 4 * t * t * t : 1 - Mathf.Pow(-2 * t + 2, 3) / 2;
    }

    public static float EaseInQuint(float t) {
        return t * t * t * t * t;
    }

    public static float EaseOutQuint(float t) {
        return 1 - Mathf.Pow(1 - t, 5);
    }

    public static float EaseInOutQuint(float t) {
        return t < 0.5f ? 16 * t * t * t * t * t : 1 - Mathf.Pow(-2 * t + 2, 5) / 2;
    }

    public static float EaseInCirc(float t) {
        return 1 - Mathf.Sqrt(1 - Mathf.Pow(t, 2));
    }

    public static float EaseOutCirc(float t) {
        return Mathf.Sqrt(1 - Mathf.Pow(t - 1, 2));
    }

    public static float EaseInOutCirc(float t) {
        return t < 0.5f
            ? (1 - Mathf.Sqrt(1 - Mathf.Pow(2 * t, 2))) / 2
            : (Mathf.Sqrt(1 - Mathf.Pow(-2 * t + 2, 2)) + 1) / 2;
    }

    public static float EaseInElastic(float t) {
        return Mathf.Sin(13 * Mathf.PI / 2 * t) * Mathf.Pow(2, 10 * (t - 1));
    }

    public static float EaseOutElastic(float t) {
        return Mathf.Sin(-13 * Mathf.PI / 2 * (t + 1)) * Mathf.Pow(2, -10 * t) + 1;
    }

    public static float EaseInOutElastic(float t) {
        return t < 0.5f
            ? Mathf.Sin(13 * Mathf.PI / 2 * (2 * t)) * Mathf.Pow(2, 10 * (2 * t - 1)) / 2
            : (Mathf.Sin(-13 * Mathf.PI / 2 * (2 * t - 1 + 1)) * Mathf.Pow(2, -10 * (2 * t - 1)) + 2) / 2;
    }

    public static float EaseInQuad(float t) {
        return t * t;
    }

    public static float EaseOutQuad(float t) {
        return 1 - (1 - t) * (1 - t);
    }

    public static float EaseInOutQuad(float t) {
        return t < 0.5f ? 2 * t * t : 1 - Mathf.Pow(-2 * t + 2, 2) / 2;
    }

    public static float EaseInQuart(float t) {
        return t * t * t * t;
    }

    public static float EaseOutQuart(float t) {
        return 1 - Mathf.Pow(1 - t, 4);
    }

    public static float EaseInOutQuart(float t) {
        return t < 0.5f ? 8 * t * t * t * t : 1 - Mathf.Pow(-2 * t + 2, 4) / 2;
    }

    public static float EaseInExpo(float t) {
        return t == 0 ? 0 : Mathf.Pow(2, 10 * t - 10);
    }

    public static float EaseOutExpo(float t) {
        return t == 1 ? 1 : 1 - Mathf.Pow(2, -10 * t);
    }

    public static float EaseInOutExpo(float t) {
        return t == 0 ? 0 :
            t == 1 ? 1 :
            t < 0.5f ? Mathf.Pow(2, 20 * t - 10) / 2 : (2 - Mathf.Pow(2, -20 * t + 10)) / 2;
    }

    public static float EaseInBack(float t) {
        return 2.70158f * t * t * t - 1.70158f * t * t;
    }

    public static float EaseOutBack(float t) {
        return 1 + 2.70158f * Mathf.Pow(t - 1, 3) + 1.70158f * Mathf.Pow(t - 1, 2);
    }

    public static float EaseInOutBack(float t) {
        return t < 0.5f
            ? Mathf.Pow(2 * t, 2) * ((2.59491f + 1) * 2 * t - 2.59491f) / 2
            : (Mathf.Pow(2 * t - 2, 2) * ((2.59491f + 1) * (t * 2 - 2) + 2.59491f) + 2) / 2;
    }

    public static float EaseInBounce(float t) {
        return 1 - EaseOutBounce(1 - t);
    }

    public static float EaseOutBounce(float t) {
        if (t < 4 / 11.0f)
            return 121 * t * t / 16.0f;
        else if (t < 8 / 11.0f)
            return 363 / 40.0f * t * t - 99 / 10.0f * t + 17 / 5.0f;
        else if (t < 9 / 10.0f)
            return 4356 / 361.0f * t * t - 35442 / 1805.0f * t + 16061 / 1805.0f;
        else
            return 54 / 5.0f * t * t - 513 / 25.0f * t + 268 / 25.0f;
    }

    public static float EaseInOutBounce(float t) {
        return t < 0.5f ? (1 - EaseOutBounce(1 - 2 * t)) / 2 : (1 + EaseOutBounce(2 * t - 1)) / 2;
    }
}

public enum EasingPreset {
    Linear,
    EaseInSine,
    EaseOutSine,
    EaseInOutSine,
    EaseInCubic,
    EaseOutCubic,
    EaseInOutCubic,
    EaseInQuint,
    EaseOutQuint,
    EaseInOutQuint,
    EaseInCirc,
    EaseOutCirc,
    EaseInOutCirc,
    EaseInElastic,
    EaseOutElastic,
    EaseInOutElastic,
    EaseInQuad,
    EaseOutQuad,
    EaseInOutQuad,
    EaseInQuart,
    EaseOutQuart,
    EaseInOutQuart,
    EaseInExpo,
    EaseOutExpo,
    EaseInOutExpo,
    EaseInBack,
    EaseOutBack,
    EaseInOutBack,
    EaseInBounce,
    EaseOutBounce,
    EaseInOutBounce
};