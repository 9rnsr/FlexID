[{Mn}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Plasma                Blood
  acc   RBC                   Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   C-bone-V              C-bone-V
  acc   T-bone-V              T-bone-V
  acc   RedMarrow             R-marrow      # only for progeny of Fe
  acc   Brain                 Brain
  acc   Kidneys               Kidneys
  acc   Liver1                Liver
  acc   Liver2                Liver
  acc   Pancreas              Pancreas
  acc   Spleen                Spleen        # only for progeny of Fe
  acc   ST0                   Other
  acc   ST1                   Other
  acc   ST2                   Other


[{Mn}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.134 p.145 Para.297
# > (297) The compartment in the iron model called ‘other plasma’ is identified with
# > the plasma compartment in the manganese model. Manganese produced in tissue
# > compartments in the model for iron is assumed to transfer to plasma with the following
# > half-times: 1 min for the blood compartment of the iron model that is not
# > included in the manganese model (plasma transferrin), 83.2 d for RBCs (based on a
# > mean lifetime of 120 d for RBCs), and 2 d for all other iron compartments (the
# > removal half-time to blood for most compartments in the characteristic model for
# > manganese). ...
# > (297) 鉄モデルにおける「その他の血漿（other plasma）」というコンパートメントは、
# > マンガンモデルにおける血漿コンパートメントと同一視されます。鉄モデルの
# > 組織コンパートメント内で生成されたマンガンは、以下の半減期で血漿へ移行すると仮定されます：
# > 鉄モデルの血液コンパートメントのうちマンガンモデルには含まれない部分（血漿トランスフェリン）
# > については1分、赤血球（RBC）については83.2日（赤血球の平均寿命120日に基づく）、
# > そしてその他のすべての鉄コンパートメントについては2日（マンガンモデルにおける大部分の
# > コンパートメントから血液への消失半減期）。 ...

  {Fe}/OtherPlasma        Plasma                    ---
  {Fe}/PlasmaTrans        Plasma                $(0.693 / (1/60/24))    # 半減期1分
  {Fe}/RBC                Plasma                $(0.693 / 83.2)         # 半減期83.2日
  {Fe}/Liver1(Hepato)     Plasma                $(0.693 / 2)            # 半減期2日
  {Fe}/Liver2(RE)         Plasma                $(0.693 / 2)            # 半減期2日
  {Fe}/Spleen             Plasma                $(0.693 / 2)            # 半減期2日
  {Fe}/MarrowSynthe       Plasma                $(0.693 / 2)            # 半減期2日
  {Fe}/MarrowTransit      Plasma                $(0.693 / 2)            # 半減期2日
  {Fe}/MarrowStorage      Plasma                $(0.693 / 2)            # 半減期2日
  {Fe}/Other1(Trans)      Plasma                $(0.693 / 2)            # 半減期2日
  {Fe}/Other2(Parenc)     Plasma                $(0.693 / 2)            # 半減期2日
  {Fe}/Other3(RE)         Plasma                $(0.693 / 2)            # 半減期2日

  {Mn}/Plasma             Plasma                    ---
  {Mn}/RBC                RBC                       ---
  {Mn}/C-bone-S           C-bone-S                  ---
  {Mn}/T-bone-S           T-bone-S                  ---
  {Mn}/C-bone-V           C-bone-V                  ---
  {Mn}/T-bone-V           T-bone-V                  ---
  {Mn}/Brain              Brain                     ---
  {Mn}/Kidneys            Kidneys                   ---
  {Mn}/Liver1             Liver1                    ---
  {Mn}/Liver2             Liver2                    ---
  {Mn}/Pancreas           Pancreas                  ---
  {Mn}/Spleen             Spleen                    ---
  {Mn}/RedMarrow          RedMarrow                 ---
  {Mn}/ST0                ST0                       ---
  {Mn}/ST1                ST1                       ---
  {Mn}/ST2                ST2                       ---

# ICRP Publ.134 p.145 Para.297
# > (297) ... Manganese is assumed to leave plasma at a rate of 1000 d⁻¹, with 30%
# > going to liver, 5% going to kidneys, 5% going to pancreas, 1% going to right colon
# > contents, 0.2% going to urinary bladder contents, 0.5% going to bone surface,
# > 0.02% going to RBCs, 0.1% going to brain, 0.3% going to spleen, 0.1% going to
# > red marrow, and the remaining 57.78% going to other soft tissue. The liver is divided
# > into two compartments: Liver 1 and Liver 2. Manganese depositing in the liver is
# > assigned to Liver 1. Manganese is removed from Liver 1 with a half-time of 1 d, with
# > 20% of outflow going to small intestine contents via biliary secretion and 80%
# > entering Liver 2. Activity transfers from Liver 2 to plasma with a half-time of 2 d.
# > Activity is removed from pancreas with a half-time of 1 d, with outflow equally
# > divided between plasma and small intestine contents. The transfer from pancreas
# > to small intestine contents represents secretion in pancreatic juice. Activity transfers
# > from kidneys to plasma with a half-time of 2 d, and from brain to plasma with a halftime
# > of 150 d. The removal half-time from RBCs is 83.2 d, as assumed for manganese
# > produced by decay of iron in RBCs. Activity depositing on bone surfaces is divided
# > equally between cortical and trabecular surfaces, and leaves bone surfaces with a
# > half-time of 40 d, with 99% returning to plasma and 1% entering the corresponding
# > bone volume compartment. Activity is removed from cortical or trabecular volume
# > at the reference turnover rate for the specific bone type in adults as given in
# > Publication 89 (ICRP, 2002). Other soft tissue is divided into ST0, ST1, and ST2
# > representing fast, intermediate, and slow turnover of manganese. ST1 receives 14.6%
# > of activity leaving plasma, ST2 receives 4%, and ST0 receives 39.18% (the amount
# > remaining after all other deposition fractions in the model were assigned). Activity is
# > returned from ST0, ST1, and ST2 to plasma with half-times of 30 min, 2 d, and 40 d,
# > respectively.
# > (297) ... マンガンは1000日⁻¹の速度で血漿から消失し、
# > その内訳は、肝臓へ30%、腎臓へ5%、膵臓へ5%、右結腸内容物へ1%、膀胱内容物へ0.2%、
# > 骨表面へ0.5%、赤血球へ0.02%、脳へ0.1%、脾臓へ0.3%、赤色骨髄へ0.1%、そして
# > 残りの57.78%がその他の軟部組織へ移行します。肝臓は「肝臓1（Liver 1）」と
# > 「肝臓2（Liver 2）」の2つのコンパートメントに分割されます。肝臓に沈着するマンガンは
# > 「肝臓1」に割り当てられます。マンガンは半減期1日で「肝臓1」から消失し、流出量の20%は
# > 胆汁分泌を介して小腸内容物へ、80%は「肝臓2」へ移行します。「肝臓2」からの移行は
# > 半減期2日で血漿へ行われます。膵臓からの消失は半減期1日で行われ、流出量は血漿と小腸内容物に
# > 均等に分配されます。膵臓から小腸内容物への移行は、膵液による分泌を表しています。
# > 腎臓から血漿への移行は半減期2日、脳から血漿への移行は半減期150日で行われます。
# > 赤血球からの消失半減期は83.2日であり、これは赤血球内の鉄の崩壊によって生成される
# > マンガンについて仮定された値と同じです。骨表面に沈着した放射能は、皮質骨表面と海綿骨表面に
# > 均等に分配され、半減期40日で骨表面から消失します。その際、99%は血漿へ戻り、1%は対応する
# > 骨体積コンパートメントへ移行します。皮質骨または海綿骨の体積からの放射能の除去は、
# > ICRP Publication 89 (ICRP, 2002) に示される成人における特定の骨型の代謝回転率に従って
# > 行われます。その他の軟部組織は、マンガンの代謝回転速度が速い、中程度、遅い順に
# > ST0、ST1、ST2に区分されます。血漿から移行する放射能のうち、ST1には14.6%、
# > ST2には4%、ST0には39.18%（モデル内の他のすべての沈着分画を割り当てた後に残る量）が
# > 分​​配されます。ST0、ST1、ST2から血漿へ戻る放射能の半減期は、それぞれ30分、2日、40日です。

  Plasma                  Liver1                $(1000 * 30%)
  Plasma                  Kidneys               $(1000 *  5%)
  Plasma                  Pancreas              $(1000 *  5%)
  Plasma                  UB-con                $(1000 *  0.2%)
  Plasma                  RC-con                $(1000 *  1%)
  Plasma                  ST0                   $(1000 * 39.18%)
  Plasma                  ST1                   $(1000 * 14.60%)
  Plasma                  ST2                   $(1000 *  4.00%)
  Plasma                  C-bone-S              $(1000 *  0.5% * 0.5)
  Plasma                  T-bone-S              $(1000 *  0.5% * 0.5)
  Plasma                  Brain                 $(1000 *  0.1%)
  Plasma                  RBC                   $(1000 *  0.02%)
  Liver1                  SI-conRe              $(0.693 / 2 * 20%)      # 半減期2日の20%
  Liver1                  Liver2                $(0.693 / 2 * 80%)      # 半減期2日の80%
  Liver2                  Plasma                $(0.693 / 2)            # 半減期2日
  Kidneys                 Plasma                $(0.693 / 2)            # 半減期2日
  Pancreas                Plasma                $(0.693 / 1 * 0.5)      # 半減期1日の50%
  Pancreas                SI-conRe              $(0.693 / 1 * 0.5)      # 半減期1日の50%
  ST0                     Plasma                $(0.693 / (30/60/24))   # 半減期30分
  ST1                     Plasma                $(0.693 / 2)            # 半減期2日
  ST2                     Plasma                $(0.693 / 40)           # 半減期40日
  C-bone-S                Plasma                $(0.693 / 40 * 99%)
  C-bone-S                C-bone-V              $(0.693 / 40 *  1%)
  T-bone-S                Plasma                $(0.693 / 40 * 99%)
  T-bone-S                T-bone-V              $(0.693 / 40 *  1%)
  C-bone-V                Plasma                $( 3% / 365)            # 成人の骨代謝回転率
  T-bone-V                Plasma                $(18% / 365)            # 成人の骨代謝回転率
  Brain                   Plasma                $(0.693 / 150)          # 半減期150日
  RBC                     Plasma                $(0.693 /  83.2)        # 半減期83.2日

  Plasma                  Spleen                $(1000 * 0.3%)
  Plasma                  RedMarrow             $(1000 * 0.1%)
  Spleen                  Plasma                $(0.693 / (30/60/24))   # ST0→Plasmaの移行速度
  RedMarrow               Plasma                $(0.693 / (30/60/24))   # ST0→Plasmaの移行速度
