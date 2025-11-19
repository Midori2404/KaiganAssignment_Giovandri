# **Character Customization System**

## **1. Overview**
### **Walkthrough Video**
https://drive.google.com/file/d/1lRPooCEgtnEHClxGzWjpZGn0Y0tbFB2v/view?usp=sharing

This project implements a modular, data-driven **Character Customization System** for runtime avatar customization, NPC crowd generation, and rendering optimization inside Unity.

It contains:
- Equipping clothing parts
- Mesh combining to reduce draw calls
- Gender-specific item catalogs
- Easy addition of new clothing assets with no code changes
- Automatic bone binding to a humanoid skeleton

---

## **2. System Architecture**

### **Main Components**

| Component | Description |
|----------|-------------|
| **AvatarCustomizeManager** | Core system: equip items, hide parts, combine meshes, manage renderer. |
| **CharacterCustomizationArea** | Controls model preview & gender selection. |
| **CharacterSpawner** | Spawns many randomized avatars for NPC/crowd testing. |
| **ClothingButtonUI** | Handles the button name and `OnClick` event |
| **ClothingDataSO** | Stores prefab, category, hidden body parts. |
| **ClothingListSO** | Contains categorized lists of all clothing items for each gender. |
| **CustomizationPartsUI** | Instantiates the clothing button depending on selected category and gender. |
| **ModelReferenceBinder** | Gender assignment, root bone, head accessories slot, base body, combined body, and hideable parts. |
| **PlayerController** | Free form movement. `WASD` for normal movement, `Q` to fly down, `E` to fly up, and hold `Left Alt` to make cursor visible. |

---

## **3. Design Decisions & Assumptions**

### **Design Decisions**
- **ScriptableObjects** was chosen to add clothing through the Inspector.
- **Mesh combining** is implemented to reduce draw calls significantly.
- **Separate male and female lists** to keep asset data clean and easy to maintain.
- **Bone auto-binding using Unity's SkinnedMeshRenderer.rootBone & bones[]** to be able to use animations.
- **Hide-body-parts** to prevent clipping and reduces overdraw.

### **This is Made in mind of:**
- All clothing meshes are exported using the **same humanoid skeleton**.
- Hair uses **GameObject prefab**; clothing uses **SkinnedMeshRenderer prefab**.
- Characters are going to be animated.

---

## **4. Rendering Optimization**

### **Technique Used: Skinned Mesh Combining**
Combined Mesh:

<img height="632" alt="image" src="https://github.com/user-attachments/assets/320d0e94-556e-434a-a6f4-fd055c043b3b" />
<img height="632" alt="image" src="https://github.com/user-attachments/assets/5b6b300c-2bba-4bae-b926-a123a5a8a86a" />

Improves runtime performance by:
- Merging multiple clothing meshes into **one combined SkinnedMeshRenderer**  
- Reusing the original rig (no re-skinning required)

### **Reason for Choosing Mesh Combining**
| Technique | Suitable for  | Chosen/Not | Reason |
|----------|--------------| ------| -------------------|
| **Skinned Mesh Combining** | Characters with multiple clothing parts | ✔ | Best for this case |
| **GPU Instancing** | Many identical static meshes | ✘ | Clothing is unique + skinned which is not supported for GPU Instancing |
| **Material Atlassing** | Large-scale productions with uniform shaders | ✘ | Requires full art pipeline and material provided is using colour instead of texture |

---

## **5. Unity Profiler Summary**

### **Before**
<img width="1107" alt="Profiler Before1" src="https://github.com/user-attachments/assets/5775bd5e-55b0-481e-8e3f-4a4d68aa8789" />
<img width="1107" alt="Profiler Before2" src="https://github.com/user-attachments/assets/5fe02bf3-1d0f-40d5-ab4f-5fe23d604b83" />

### **After**
<img width="1107" alt="Profiler After2" src="https://github.com/user-attachments/assets/232add1d-018c-4f52-9ddb-3802e1f5380a" />
<img width="1107" alt="Profiler After1" src="https://github.com/user-attachments/assets/fd47858b-2499-427d-86dc-22dc51f05ed8" />

## **6. Unity Frame Debugger Summary**

### **Before**
<img width="1919" alt="Frame Debugger Before" src="https://github.com/user-attachments/assets/c7ff1f02-db08-4f43-8b0a-3fc0d6538852" />

### **After**
<img width="1919" alt="Frame Debugger After" src="https://github.com/user-attachments/assets/f117b981-3c38-427e-82bd-57b76a51628b" />

---

## **6. Extending the System**

### **Adding a New Clothing Item**
1. Create a new `ClothingDataSO`
2. Assign:
   - Clothing prefab
   - Category
   - Hidden body parts name (string should match with Body Parts in prefab with ModelReferenceBinder)
3. Add the new clothing into the correct `ClothingListSO` category
4. Add UI button to call `EquipClothing`
5. Assign new clothing as the parameter

### **Adding a New Category**
1. Add an enum entry to `ClothingCategory`
2. Add a UI button for new category
3. Add a list in `ClothingListSO`

Minimal changes required due to the data-driven structure.

---

## **7. What I Would Add With More Time**
- Save/Load system for player appearance
- Material atlassing tool for automated baking
- LOD system for optimized distant NPCs
- GPU instancing support for large crowds by using 3rd Party Tools

---

## **8. Bonus Questions**

### **1. How would you profile and optimize this for low-end mobile devices?**
- Profile on an actual low-end device using Unity Profiler (CPU, GPU, rendering, memory).
- Use **Frame Debugger** to check draw calls and material switches.
- Adding **LOD levels** for far characters.
- Ensure no unnecessary GameObjects, scripts, or updates run per frame.

### **2. What tooling or pipelines would you create for designers?**
- **Clothing Import Tool**  
  Creates ClothingDataSO from an FBX file.

- **Catalog Preview Window**  
  Shows all clothing items as thumbnails so designers can quickly check assets.
