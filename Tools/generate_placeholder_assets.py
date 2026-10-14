#!/usr/bin/env python3
"""
Endless Rails - Procedural Placeholder Asset Generator
======================================================
Generates CC0/original placeholder PBR-ish textures and synthesized sound effects
so the Unity project is fully playable out of the box without third-party assets.

Everything generated here is 100% original, procedurally created content
released under CC0 1.0 (public domain) by this script's author.

Usage:  python3 Tools/generate_placeholder_assets.py
Output: Assets/Textures/Generated/*.png
        Assets/Audio/Generated/*.wav
"""

import os
import math
import wave
import struct
import numpy as np
from PIL import Image, ImageFilter

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
TEX_DIR = os.path.join(ROOT, "Assets", "Textures", "Generated")
AUD_DIR = os.path.join(ROOT, "Assets", "Audio", "Generated")
os.makedirs(TEX_DIR, exist_ok=True)
os.makedirs(AUD_DIR, exist_ok=True)

rng = np.random.default_rng(1337)


# ----------------------------------------------------------------------------
# Texture helpers
# ----------------------------------------------------------------------------

def value_noise(size, freq, octaves=4, persistence=0.5, seed=0):
    """Fractal value noise in [0,1]."""
    r = np.random.default_rng(seed)
    total = np.zeros((size, size), dtype=np.float64)
    amp = 1.0
    norm = 0.0
    f = freq
    for _ in range(octaves):
        grid = r.random((max(2, int(f)), max(2, int(f))))
        img = Image.fromarray((grid * 255).astype(np.uint8)).resize((size, size), Image.BICUBIC)
        total += np.asarray(img, dtype=np.float64) / 255.0 * amp
        norm += amp
        amp *= persistence
        f *= 2
    return total / norm


def normal_from_height(height, strength=2.0):
    """Convert a height map [0,1] to a tangent-space normal map RGB image."""
    h = height * strength
    gx, gy = np.gradient(h)
    nx, ny, nz = -gx, -gy, np.ones_like(h)
    length = np.sqrt(nx * nx + ny * ny + nz * nz)
    nx, ny, nz = nx / length, ny / length, nz / length
    rgb = np.stack([(nx * 0.5 + 0.5), (ny * 0.5 + 0.5), (nz * 0.5 + 0.5)], axis=-1)
    return Image.fromarray((rgb * 255).astype(np.uint8), "RGB")


def save_pbr(name, albedo, height, rough_base=0.8, metal=0.0):
    """Save albedo + normal + a packed-ish mask (R=roughness, G=AO, B=metallic)."""
    albedo = np.clip(albedo, 0, 1)
    Image.fromarray((albedo * 255).astype(np.uint8), "RGB").save(
        os.path.join(TEX_DIR, f"{name}_albedo.png"))
    normal_from_height(height).save(os.path.join(TEX_DIR, f"{name}_normal.png"))
    ao = np.clip(0.55 + 0.45 * height, 0, 1)
    rough = np.clip(rough_base + 0.25 * (height - 0.5), 0, 1)
    mask = np.stack([rough, ao, np.full_like(height, metal)], axis=-1)
    Image.fromarray((mask * 255).astype(np.uint8), "RGB").save(
        os.path.join(TEX_DIR, f"{name}_mask.png"))
    print(f"  texture: {name}")


def tint(gray, color):
    c = np.array(color, dtype=np.float64) / 255.0
    return gray[..., None] * c[None, None, :]


# ----------------------------------------------------------------------------
# Textures
# ----------------------------------------------------------------------------

def gen_concrete():
    n = value_noise(512, 6, 5, 0.55, seed=11)
    speck = (rng.random((512, 512)) > 0.996).astype(np.float64)
    height = np.clip(n * 0.85 + speck * 0.3, 0, 1)
    base = 0.42 + 0.22 * n
    stains = value_noise(512, 3, 3, 0.5, seed=12)
    gray = np.clip(base - 0.10 * stains, 0, 1)
    albedo = tint(gray, (198, 196, 191))
    save_pbr("concrete", albedo, height, rough_base=0.85)


def gen_gravel():
    n = value_noise(512, 24, 4, 0.5, seed=21)
    stones = (rng.random((512, 512)) > 0.92).astype(np.float64)
    stones = np.asarray(Image.fromarray((stones * 255).astype(np.uint8)).filter(
        ImageFilter.MaxFilter(5)), dtype=np.float64) / 255.0
    height = np.clip(n * 0.7 + stones * 0.5, 0, 1)
    gray = 0.30 + 0.35 * height
    albedo = tint(gray, (132, 122, 110))
    save_pbr("gravel", albedo, height, rough_base=0.95)


def gen_ballast():
    n = value_noise(512, 14, 4, 0.55, seed=31)
    crack = np.abs(np.sin(value_noise(512, 5, 3, 0.5, seed=32) * 24.0))
    crack = np.clip(1.0 - crack, 0, 1) ** 3
    height = np.clip(n * 0.9 - crack * 0.4, 0, 1)
    gray = 0.26 + 0.30 * height
    albedo = tint(gray, (110, 104, 98))
    save_pbr("ballast", albedo, height, rough_base=0.92)


def gen_metal_brushed():
    yy, xx = np.mgrid[0:512, 0:512]
    lines = 0.5 + 0.5 * np.sin(yy * 0.9 + value_noise(512, 8, 3, 0.5, seed=41) * 12)
    n = value_noise(512, 4, 3, 0.5, seed=42)
    height = np.clip(0.5 + 0.25 * lines + 0.25 * n, 0, 1)
    gray = 0.55 + 0.20 * lines + 0.10 * n
    albedo = tint(gray, (186, 190, 196))
    save_pbr("metal_brushed", albedo, height, rough_base=0.38, metal=1.0)


def gen_rust():
    n = value_noise(512, 8, 5, 0.55, seed=51)
    blotch = value_noise(512, 3, 4, 0.6, seed=52)
    height = np.clip(n * 0.8 + blotch * 0.3, 0, 1)
    gray = 0.35 + 0.35 * n
    albedo = tint(gray, (150, 82, 48)) + tint(blotch * 0.5, (96, 55, 38))
    save_pbr("rust", albedo, height, rough_base=0.9, metal=0.35)


def gen_asphalt():
    n = value_noise(512, 18, 5, 0.5, seed=61)
    grain = rng.random((512, 512)) * 0.18
    height = np.clip(n * 0.8 + grain, 0, 1)
    gray = 0.16 + 0.16 * n
    albedo = tint(gray, (120, 120, 125))
    save_pbr("asphalt", albedo, height, rough_base=0.88)


def gen_graffiti():
    base = value_noise(512, 5, 4, 0.5, seed=71) * 0.25 + 0.45
    albedo = tint(base, (210, 208, 202))
    r = np.random.default_rng(72)
    img = Image.fromarray((np.clip(albedo, 0, 1) * 255).astype(np.uint8), "RGB")
    # paint splat blobs
    for _ in range(14):
        cx, cy = r.integers(40, 472, 2)
        rad = int(r.integers(18, 70))
        color = tuple(int(v) for v in r.integers(20, 255, 3))
        layer = Image.new("RGB", (512, 512))
        for y in range(max(0, cy - rad), min(512, cy + rad)):
            for x in range(max(0, cx - rad), min(512, cx + rad)):
                if (x - cx) ** 2 + (y - cy) ** 2 < rad * rad:
                    layer.putpixel((x, y), color)
        img = Image.blend(img, layer, 0.5)
    img = img.filter(ImageFilter.GaussianBlur(1.2))
    albedo = np.asarray(img, dtype=np.float64) / 255.0
    height = value_noise(512, 10, 3, 0.5, seed=73)
    save_pbr("graffiti", albedo, height, rough_base=0.75)


def gen_wood():
    yy, xx = np.mgrid[0:512, 0:512]
    rings = 0.5 + 0.5 * np.sin(xx / 6.0 + value_noise(512, 4, 3, 0.5, seed=81) * 9)
    height = np.clip(rings, 0, 1)
    gray = 0.35 + 0.25 * rings
    albedo = tint(gray, (168, 124, 82))
    save_pbr("wood", albedo, height, rough_base=0.8)


def gen_platform_tile():
    yy, xx = np.mgrid[0:512, 0:512]
    tile = ((xx // 64) + (yy // 64)) % 2
    n = value_noise(512, 8, 4, 0.5, seed=91)
    height = np.clip(0.35 + 0.3 * tile + 0.35 * n, 0, 1)
    gray = 0.48 + 0.12 * tile + 0.14 * n
    albedo = tint(gray, (186, 182, 174))
    save_pbr("platform_tile", albedo, height, rough_base=0.82)


def gen_train_paint():
    n = value_noise(512, 6, 4, 0.5, seed=101)
    scratches = (value_noise(512, 40, 2, 0.5, seed=102) > 0.82).astype(np.float64)
    height = np.clip(n * 0.6 + scratches * 0.4, 0, 1)
    gray = 0.62 + 0.12 * n
    albedo = tint(gray, (210, 140, 40))  # warm orange/yellow livery (original design)
    save_pbr("train_paint", albedo, height, rough_base=0.42, metal=0.25)


def gen_wet_puddle_mask():
    n = value_noise(512, 3, 3, 0.5, seed=111)
    mask = np.clip((n - 0.55) * 4, 0, 1)
    rgb = np.stack([mask, np.full_like(mask, 1.0), np.zeros_like(mask)], axis=-1)
    Image.fromarray((rgb * 255).astype(np.uint8), "RGB").save(
        os.path.join(TEX_DIR, "puddle_mask.png"))
    print("  texture: puddle_mask")


# ----------------------------------------------------------------------------
# Audio helpers
# ----------------------------------------------------------------------------

SR = 44100


def save_wav(name, data, sr=SR):
    data = np.clip(data, -1.0, 1.0)
    pcm = (data * 32767).astype(np.int16)
    with wave.open(os.path.join(AUD_DIR, f"{name}.wav"), "wb") as w:
        w.setnchannels(1)
        w.setsampwidth(2)
        w.setframerate(sr)
        w.writeframes(pcm.tobytes())
    print(f"  audio: {name}")


def envelope(n, attack=0.01, release=0.3, sustain=1.0):
    env = np.ones(n) * sustain
    a = max(1, int(n * attack))
    r = max(1, int(n * release))
    env[:a] = np.linspace(0, 1, a)
    env[-r:] *= np.linspace(1, 0, r)
    return env


def lowpass(x, alpha=0.2):
    y = np.empty_like(x)
    acc = 0.0
    for i in range(len(x)):
        acc += alpha * (x[i] - acc)
        y[i] = acc
    return y


def noise(n):
    return rng.uniform(-1, 1, n)


def tone(freq, dur, sr=SR, harmonics=(1.0, 0.4, 0.2), decay=None):
    n = int(dur * sr)
    t = np.arange(n) / sr
    out = np.zeros(n)
    for i, h in enumerate(harmonics):
        out += h * np.sin(2 * math.pi * freq * (i + 1) * t)
    out /= sum(harmonics)
    if decay is not None:
        out *= np.exp(-t * decay)
    return out


# ----------------------------------------------------------------------------
# Audio
# ----------------------------------------------------------------------------

def gen_coin():
    n = int(0.45 * SR)
    t = np.arange(n) / SR
    s = (np.sin(2 * math.pi * 1870 * t) * 0.5 +
         np.sin(2 * math.pi * 2510 * t) * 0.3 +
         np.sin(2 * math.pi * 3720 * t) * 0.18 +
         np.sin(2 * math.pi * 5230 * t) * 0.10)
    s *= np.exp(-t * 9.0)
    click = noise(int(0.01 * SR)) * 0.5
    s[:len(click)] += click
    save_wav("coin_pickup", s)


def gen_coin_multi():
    base = np.zeros(int(0.8 * SR))
    for i, d in enumerate([0.0, 0.07, 0.14]):
        seg = np.zeros_like(base)
        n = int(0.3 * SR)
        t = np.arange(n) / SR
        seg[int(d * SR):int(d * SR) + n] = (
            np.sin(2 * math.pi * (1870 + i * 320) * t) * 0.4 +
            np.sin(2 * math.pi * (2510 + i * 410) * t) * 0.25) * np.exp(-t * 10)
        base += seg
    save_wav("coin_multi", base)


def gen_footstep(surface):
    n = int(0.16 * SR)
    t = np.arange(n) / SR
    body = np.sin(2 * math.pi * 95 * t) * np.exp(-t * 42) * 0.9
    if surface == "concrete":
        scuff = lowpass(noise(n), 0.35) * np.exp(-t * 30) * 0.5
    elif surface == "gravel":
        scuff = (noise(n) * (rng.random(n) > 0.6)) * np.exp(-t * 16) * 0.7
        scuff = lowpass(scuff, 0.5)
    elif surface == "metal":
        ring = (np.sin(2 * math.pi * 420 * t) + np.sin(2 * math.pi * 780 * t) * 0.5) * np.exp(-t * 22) * 0.35
        scuff = lowpass(noise(n), 0.6) * np.exp(-t * 45) * 0.35 + ring
    else:  # wet
        scuff = lowpass(noise(n), 0.45) * np.exp(-t * 12) * 0.4
        out = body * 0.7 + scuff
        # splash tail appended (never longer than the output buffer)
        n2 = min(int(0.22 * SR), len(out))
        t2 = np.arange(n2) / SR
        splash = lowpass(noise(n2), 0.5) * np.exp(-t2 * 9) * 0.25
        out[:n2] += splash
        save_wav("footstep_wet", out)
        return
    save_wav(f"footstep_{surface}", body * 0.8 + scuff)


def gen_jump():
    n = int(0.35 * SR)
    t = np.arange(n) / SR
    sweep = noise(n) * 0.35
    # rising whoosh via amplitude shape + lowpass that opens
    sweep = sweep * np.linspace(0.2, 1.0, n)
    body = np.sin(2 * math.pi * (140 + 260 * t / 0.35) * t) * 0.35 * np.exp(-t * 6)
    save_wav("jump_whoosh", sweep * np.hanning(n) + body)


def gen_land():
    n = int(0.4 * SR)
    t = np.arange(n) / SR
    thud = np.sin(2 * math.pi * (70 - 120 * t) * t) * np.exp(-t * 14)
    crack = lowpass(noise(n), 0.4) * np.exp(-t * 32) * 0.55
    save_wav("land_thud", thud * 0.9 + crack)


def gen_slide():
    n = int(0.9 * SR)
    t = np.arange(n) / SR
    scrape = lowpass(noise(n), 0.55)
    scrape *= (np.exp(-t * 2.2) * 0.55 + 0.15 * np.sin(2 * math.pi * 7 * t) ** 2)
    save_wav("slide_scrape", scrape)


def gen_crash():
    n = int(1.2 * SR)
    t = np.arange(n) / SR
    boom = np.sin(2 * math.pi * (58 - 30 * t) * t) * np.exp(-t * 3.2)
    metal = lowpass(noise(n), 0.7) * np.exp(-t * 5.5) * 0.7
    shards = (noise(n) * (rng.random(n) > 0.985)) * np.exp(-t * 3.0) * 0.8
    save_wav("crash", boom * 1.0 + metal + shards)


def gen_stumble():
    n = int(0.5 * SR)
    t = np.arange(n) / SR
    s = lowpass(noise(n), 0.5) * np.exp(-t * 9) * 0.6
    s += np.sin(2 * math.pi * 130 * t) * np.exp(-t * 12) * 0.4
    save_wav("stumble", s)


def gen_train_horn():
    n = int(1.6 * SR)
    t = np.arange(n) / SR
    a = np.sin(2 * math.pi * 233 * t) + 0.65 * np.sin(2 * math.pi * 294 * t) + 0.4 * np.sin(2 * math.pi * 349 * t)
    env = np.minimum(1, t * 20) * np.minimum(1, (1.6 - t) * 6)
    s = a * env * 0.35
    # slight vibrato
    s *= 1 + 0.05 * np.sin(2 * math.pi * 5.5 * t)
    save_wav("train_horn", s)


def gen_train_rumble():
    n = int(3.0 * SR)
    s = lowpass(noise(n), 0.12) * 0.8
    t = np.arange(n) / SR
    s += np.sin(2 * math.pi * 42 * t) * 0.15
    s += np.sin(2 * math.pi * 63 * t) * 0.08
    s *= 0.5 + 0.5 * np.sin(2 * math.pi * 2.2 * t) ** 2  # wheel clack modulation
    save_wav("train_rumble", s)


def gen_wind():
    n = int(4.0 * SR)
    s = lowpass(noise(n), 0.08)
    t = np.arange(n) / SR
    s *= 0.35 + 0.25 * np.sin(2 * math.pi * 0.13 * t) + 0.1 * np.sin(2 * math.pi * 0.31 * t + 1.2)
    save_wav("wind_ambient", s * 0.8)


def gen_rain():
    n = int(4.0 * SR)
    drops = noise(n) * (rng.random(n) > 0.992)
    hiss = lowpass(noise(n), 0.55) * 0.35
    s = lowpass(drops * 6, 0.6) * 0.6 + hiss
    save_wav("rain_ambient", s * 0.75)


def gen_city_ambient():
    n = int(5.0 * SR)
    t = np.arange(n) / SR
    s = lowpass(noise(n), 0.05) * 0.5
    s += np.sin(2 * math.pi * 55 * t) * 0.06
    s += np.sin(2 * math.pi * 110 * t + np.sin(t)) * 0.03
    # occasional distant honk
    for start in (1.2, 3.4):
        i0 = int(start * SR)
        m = int(0.35 * SR)
        tt = np.arange(m) / SR
        s[i0:i0 + m] += np.sin(2 * math.pi * 392 * tt) * np.exp(-tt * 4) * 0.12
    save_wav("city_ambient", s)


def gen_button():
    n = int(0.12 * SR)
    t = np.arange(n) / SR
    s = (np.sin(2 * math.pi * 660 * t) + 0.5 * np.sin(2 * math.pi * 990 * t)) * np.exp(-t * 30)
    save_wav("button_click", s * 0.5)


def gen_powerup():
    n = int(0.8 * SR)
    t = np.arange(n) / SR
    freq = 330 * (2 ** (t / 0.8 * 1.5))
    phase = 2 * math.pi * np.cumsum(freq) / SR
    s = (np.sin(phase) + 0.4 * np.sin(2 * phase)) * envelope(n, 0.02, 0.25) * 0.45
    save_wav("powerup", s)


def gen_powerdown():
    n = int(0.6 * SR)
    t = np.arange(n) / SR
    freq = 660 * (2 ** (-t / 0.6 * 1.5))
    phase = 2 * math.pi * np.cumsum(freq) / SR
    s = np.sin(phase) * envelope(n, 0.01, 0.3) * 0.4
    save_wav("powerdown", s)


def gen_shield_break():
    n = int(0.7 * SR)
    t = np.arange(n) / SR
    s = np.sin(2 * math.pi * 520 * t) * np.exp(-t * 6) * 0.4
    s += lowpass(noise(n), 0.7) * np.exp(-t * 8) * 0.5
    save_wav("shield_break", s)


def gen_shout():
    # Formant-ish synthesized "HEY!" placeholder bark
    n = int(0.55 * SR)
    t = np.arange(n) / SR
    f0 = 165 * np.exp(-t * 1.2) + 120
    phase = 2 * math.pi * np.cumsum(f0) / SR
    voiced = np.zeros(n)
    for formant, amp, bw in ((600, 1.0, 90), (1100, 0.7, 120), (2400, 0.35, 160)):
        voiced += amp * np.sin(phase * (formant / 120.0)) * np.exp(-((f0 - formant) ** 2) / (bw * 200))
    voiced = voiced / (np.max(np.abs(voiced)) + 1e-6)
    env = envelope(n, 0.06, 0.28)
    grit = lowpass(noise(n), 0.6) * 0.25
    save_wav("shout", (voiced * env + grit * env) * 0.55)


def gen_chaser_steps():
    # double footstep loop (gravel-ish)
    n = int(0.5 * SR)
    s = np.zeros(n)
    for d in (0.0, 0.22):
        i0 = int(d * SR)
        m = int(0.18 * SR)
        t = np.arange(m) / SR
        s[i0:i0 + m] += (np.sin(2 * math.pi * 88 * t) * np.exp(-t * 30) * 0.7 +
                         lowpass(noise(m), 0.45) * np.exp(-t * 18) * 0.5)
    save_wav("chaser_steps", s)


def gen_ui_swoosh():
    n = int(0.4 * SR)
    t = np.arange(n) / SR
    s = lowpass(noise(n), 0.3) * np.sin(math.pi * t / 0.4) * 0.4
    save_wav("ui_swoosh", s)


def gen_game_over():
    n = int(1.8 * SR)
    t = np.arange(n) / SR
    s = np.zeros(n)
    for f, d in ((392, 0.0), (349, 0.35), (294, 0.7), (220, 1.05)):
        i0 = int(d * SR)
        m = min(int(0.5 * SR), n - i0)
        tt = np.arange(m) / SR
        s[i0:i0 + m] += np.sin(2 * math.pi * f * tt) * np.exp(-tt * 3.2) * 0.32
        s[i0:i0 + m] += np.sin(2 * math.pi * f * 2 * tt) * np.exp(-tt * 4.5) * 0.12
    save_wav("game_over", s)


def gen_music():
    """Simple 8-bar loop: bass + pad + hats. Original composition, CC0."""
    bpm = 116
    beat = 60.0 / bpm
    bars = 8
    n = int(bars * 4 * beat * SR)
    t = np.arange(n) / SR
    s = np.zeros(n)

    def note(freq, start, dur, amp=0.2, kind="saw"):
        i0 = int(start * SR)
        m = min(int(dur * SR), n - i0)
        if m <= 0:
            return
        tt = np.arange(m) / SR
        if kind == "saw":
            w = np.zeros(m)
            for h in range(1, 8):
                w += np.sin(2 * math.pi * freq * h * tt) / h
            w *= 0.35
        elif kind == "sine":
            w = np.sin(2 * math.pi * freq * tt)
        else:  # pad
            w = (np.sin(2 * math.pi * freq * tt) +
                 0.6 * np.sin(2 * math.pi * freq * 1.004 * tt) +
                 0.4 * np.sin(2 * math.pi * freq * 0.996 * tt)) / 2
        w *= amp * envelope(m, 0.02, 0.25)
        s[i0:i0 + m] += w

    # Am - F - C - G progression
    roots = [110.0, 87.3, 130.8, 98.0]
    chords = [(220, 261.6, 329.6), (174.6, 220, 261.6), (261.6, 329.6, 392), (196, 246.9, 293.7)]
    for bar in range(bars):
        start = bar * 4 * beat
        r = roots[bar % 4]
        for b in range(4):
            note(r, start + b * beat, beat * 0.9, 0.22, "saw")
        for f in chords[bar % 4]:
            note(f, start, 4 * beat, 0.075, "pad")
        # hats
        for b in range(8):
            i0 = int((start + b * beat * 0.5) * SR)
            m = int(0.03 * SR)
            if i0 + m < n:
                s[i0:i0 + m] += noise(m) * np.exp(-np.arange(m) / SR * 90) * 0.12
        # kick
        for b in (0, 2):
            i0 = int((start + b * beat) * SR)
            m = int(0.12 * SR)
            if i0 + m < n:
                tt = np.arange(m) / SR
                s[i0:i0 + m] += np.sin(2 * math.pi * (90 - 120 * tt) * tt) * np.exp(-tt * 16) * 0.5
    s *= 0.85
    save_wav("music_loop", s)


# ----------------------------------------------------------------------------
# Main
# ----------------------------------------------------------------------------

def main():
    print("Generating textures ...")
    gen_concrete()
    gen_gravel()
    gen_ballast()
    gen_metal_brushed()
    gen_rust()
    gen_asphalt()
    gen_graffiti()
    gen_wood()
    gen_platform_tile()
    gen_train_paint()
    gen_wet_puddle_mask()

    print("Generating audio ...")
    gen_coin()
    gen_coin_multi()
    for surf in ("concrete", "gravel", "metal", "wet"):
        gen_footstep(surf)
    gen_jump()
    gen_land()
    gen_slide()
    gen_crash()
    gen_stumble()
    gen_train_horn()
    gen_train_rumble()
    gen_wind()
    gen_rain()
    gen_city_ambient()
    gen_button()
    gen_powerup()
    gen_powerdown()
    gen_shield_break()
    gen_shout()
    gen_chaser_steps()
    gen_ui_swoosh()
    gen_game_over()
    gen_music()
    print("Done. Assets written to Assets/Textures/Generated and Assets/Audio/Generated")


if __name__ == "__main__":
    main()
