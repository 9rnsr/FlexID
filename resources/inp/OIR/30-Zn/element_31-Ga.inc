[{Ga}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood                 Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   C-marrow              C-marrow
  acc   T-marrow              T-marrow
  acc   Liver                 Liver
  acc   Kidneys               Kidneys
  acc   Pancreas              Pancreas
  acc   Spleen                Spleen
  acc   SkeletalMuscle        Muscle
  acc   ST0                   Other
  acc   ST1                   Other


[{Ga}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.134 p.201 Para.437
# > (437) ... Blood in the gallium model is identified with the plasma compartment
# > of the zinc model. Gallium produced in compartments of the systemic model for zinc
# > (Fig. 9.1) other than plasma are assumed to be transferred to the blood compartment
# > of the gallium model with the following half-times: 1 min for RBCs; 5 d for liver,
# > spleen, pancreas, and muscle; 0.5 d for kidneys and compartments of other soft
# > tissue; 2 d for bone surface compartments; and the bone turnover half-time for
# > bone volume compartments. The subsequent behaviour of gallium that reaches the
# > blood compartment is determined by the gallium model described above.
# > (437) ...ガリウムモデルにおける血液は、亜鉛モデルの血漿コンパートメントと同一視される。
# > 亜鉛全身モデル（図9.1）の血漿以外のコンパートメントで生成されたガリウムは、以下の半減期で
# > ガリウムモデルの血液コンパートメントに移行すると仮定する。赤血球では1分、
# > 肝臓、脾臓、膵臓、筋肉では5日、腎臓およびその他の軟部組織コンパートメントでは0.5日、
# > 骨表面コンパートメントでは2日、骨体積コンパートメントでは骨代謝の半減期である。
# > 血液コンパートメントに到達したガリウムのその後の挙動は、上記のガリウムモデルによって決定される。

  {Zn}/Plasma             Blood                     ---
  {Zn}/RBC                Blood                 $(0.693 / (1/60/24))    # 半減期1分
  {Zn}/Liver1             Blood                 $(0.693 / 5)            # 半減期5日
  {Zn}/Liver2             Blood                 $(0.693 / 5)            # 半減期5日
 #{Zn}/Spleen             Blood                 $(0.693 / 5)            # 半減期5日     # Znモデルに脾臓はない
  {Zn}/Pancreas           Blood                 $(0.693 / 5)            # 半減期5日
  {Zn}/Muscle             Blood                 $(0.693 / 5)            # 半減期5日
  {Zn}/Kidneys            Blood                 $(0.693 / 0.5)          # 半減期0.5日
  {Zn}/ST0                Blood                 $(0.693 / 0.5)          # 半減期0.5日
  {Zn}/ST1                Blood                 $(0.693 / 0.5)          # 半減期0.5日
  {Zn}/ST2                Blood                 $(0.693 / 0.5)          # 半減期0.5日
  {Zn}/C-bone-S           Blood                 $(0.693 / 2)            # 半減期2日
  {Zn}/T-bone-S           Blood                 $(0.693 / 2)            # 半減期2日
  {Zn}/C-bone-V           Blood                 $( 3% / 365)            # 成人の骨代謝回転率
  {Zn}/T-bone-V           Blood                 $(18% / 365)            # 成人の骨代謝回転率

# ICRP Publ.134 p.201 Para.437
# > (437) ... The gallium model includes compartments representing blood,
# > liver, kidneys, spleen, pancreas, muscle, trabecular bone surface, trabecular bone
# > marrow, cortical bone surface, and cortical bone marrow, and two compartments
# > representing other soft tissue. Gallium is assumed to leave blood at a rate of 5 d⁻¹,
# > with 20% depositing on bone surface, 10% in marrow, 6% in liver, 8% in kidneys,
# > 4% in muscle, 1% in spleen, 0.1% in pancreas, 3% in right colon contents, 10% in a
# > soft tissue compartment with relatively slow transfer back to blood (t½=1 y), and the
# > remainder (37.9%) in a soft tissue compartment with relatively fast transfer back to
# > blood (t½=0.5 d). The bone and marrow deposits are assumed to be equally divided
# > between trabecular and cortical bone. Gallium is removed from liver, spleen, pancreas,
# > and muscle to blood with a half-time of 5 d; from kidneys to urinary bladder
# > contents with a half-time of 0.5 d; and from bone surface and marrow to blood with a
# > half-time of 2 d.  ...
# > (437) ... ガリウムモデルは、血液、肝臓、腎臓、脾臓、膵臓、筋肉、海綿骨表面、海綿骨髄、
# > 皮質骨表面、皮質骨髄を表すコンパートメントと、その他の軟部組織を表す2つのコンパートメントから
# > 構成される。ガリウムは5 d⁻¹の速度で血液から消失すると仮定され、20%が骨表面に沈着し、
# > 10%が骨髄に、6%が肝臓に、8%が腎臓に、4%が筋肉に、1%が脾臓に、0.1%が膵臓に、3%が右結腸内容物に
# > 沈着し、10%が比較的ゆっくりと血液に戻る軟部組織区画（t½=1年）に沈着し、残り（37.9%）は
# > 比較的速く血液に戻る軟部組織区画（t½=0.5日）に沈着する。骨髄への沈着は、海綿骨と皮質骨に
# > 均等に分配されると仮定される。ガリウムは、肝臓、脾臓、膵臓、筋肉から血液へは半減期5日で、
# > 腎臓から膀胱内容物へは半減期0.5日で沈着する。骨表面および骨髄から血液へは、半減期2日で移行する。
# > ...

  Blood                   Liver                 $(5 *  6%)                          # == 0.3
  Blood                   Kidneys               $(5 *  8%)                          # == 0.4
  Blood                   T-bone-S              $(5 * 20% * 0.5)                    # == 0.5
  Blood                   C-bone-S              $(5 * 20% * 0.5)                    # == 0.5
  Blood                   T-marrow              $(5 * 10% * 0.5)                    # == 0.25   # <--
  Blood                   C-marrow              $(5 * 10% * 0.5)                    # == 0.25   # <--
  Blood                   SkeletalMuscle        $(5 *  4%)                          # == 0.2
  Blood                   Spleen                $(5 *  1%)                          # == 0.05
  Blood                   Pancreas              $(5 *  0.1%)                        # == 0.005
  Blood                   RC-con                $(5 *  3%)                          # == 0.15
  Blood                   ST0                   $(5 * 37.9%)                        # == 1.895  # <--
  Blood                   ST1                   $(5 * 10%)                          # == 0.5
  ST0                     Blood                 $(0.693 / 0.5)      # 半減期0.5日   # == 1.39
  ST1                     Blood                 $(0.693 / 365)      # 半減期1年     # == 0.0019
  Liver                   Blood                 $(0.693 / 5)        # 半減期5日     # == 0.139
  Spleen                  Blood                 $(0.693 / 5)        # 半減期5日     # == 0.139
  Pancreas                Blood                 $(0.693 / 5)        # 半減期5日     # == 0.139
  SkeletalMuscle          Blood                 $(0.693 / 5)        # 半減期5日     # == 0.139
  Kidneys                 UB-con                $(0.693 / 0.5)      # 半減期0.5日   # == 1.39
  T-bone-S                Blood                 $(0.693 / 2)        # 半減期2日     # == 0.347
  C-bone-S                Blood                 $(0.693 / 2)        # 半減期2日     # == 0.347
  T-marrow                Blood                 $(0.693 / 2)        # 半減期2日     # == 0.347
  C-marrow                Blood                 $(0.693 / 2)        # 半減期2日     # == 0.347

# インプット作成者注：Publ.134 Para.437 と Publ.151 Table 17.3 を比較すると、
# - Publ.134では骨髄が海綿骨と皮質骨に均等に分かれていると仮定されているのに対して、
#   Publ.151では赤色骨髄に纏められている。
# - 上記を反映してか、血液から骨髄への流出が、
#   Publ.134では 5 d⁻¹ * 10% = 0.5 d⁻¹ なのに対して、
#   Publ.151では 0.25 d⁻¹ になっており、半分の速度に設定されている。
# - また骨髄への流出速度低下を補填するように、血液からST0(比較的早く血液に戻る軟部組織区画)への流出が、
#   Publ.134では 5 * 37.9% = 1.895 d⁻¹ なのに対して、
#   Publ.151では 2.145 d⁻¹ = 1.895 + 0.25 d⁻¹ と速度増加されている。

# # ICRP Publ.151 p.164 Table 17.3
#   Blood                   RC-con                    0.15
#   Blood                   Liver                     0.3
#   Blood                   Kidneys                   0.4
#   Blood                   Spleen                    0.05
#   Blood                   T-bone-S                  0.5
#   Blood                   C-bone-S                  0.5
#   Blood                   RedMarrow                 0.25      # <--
#   Blood                   SkeletalMuscle            0.2
#   Blood                   Pancreas                  0.005
#   Blood                   ST0                       2.145     # <--
#   Blood                   ST1                       0.5
#   Liver                   Blood                     0.139
#   Kidneys                 UB-con                    1.39
#   Spleen                  Blood                     0.139
#   T-bone-S                Blood                     0.347
#   C-bone-S                Blood                     0.347
#   RedMarrow               Blood                     0.347
#   SkeletalMuscle          Blood                     0.139
#   Pancreas                Blood                     0.139
#   ST0                     Blood                     1.39
#   ST1                     Blood                     0.0019
