# ICRP Publ.134 p.224 Para.485
# > (485) The model for krypton produced by serial decay of strontium and rubidium
# > in systemic compartments is similar to the model applied in the OIR series to radon
# > produced in vivo by decay of a parent radionuclide (ICRP, 2017, see radon section in
# > OIR Part 3). ...
#  > (485) 全身コンパートメント内でのストロンチウムおよびルビジウムの逐次壊変によって生成される
#  > クリプトンに関するモデルは、親放射性核種の壊変により生体内で生成されるラドンに対して
#  > OIRシリーズで適用されたモデルと類似しています（ICRP, 2017；OIRパート3のラドンの項を参照）。 ...

[{Kr}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood                 Blood
  exc   Exhaled               ---


[{Kr}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.134 p.224 Para.485
# > (485) ... Krypton is assumed to follow the bone model for radon introduced in
# > Publication 67 (ICRP, 1993), but is assigned a higher rate of removal from soft tissues
# > to blood than is assumed for radon. Specifically, krypton produced in
# > non-exchangeable bone volume, exchangeable bone volume, or bone surface transfers
# > to blood at rates of 0.36 d⁻¹, 1.5 d⁻¹, or 100 d⁻¹, respectively. Krypton produced in a
# > soft tissue compartment transfers to blood with a half-time of 15 min, compared with
# > an assumed half-time of 30 min for radon produced by radioactive decay in soft
# > tissues. Krypton entering blood is assumed to be removed from the body (exhaled)
# > at a rate of 1000 d⁻¹, corresponding to a half-time of 1 min. Recycling of krypton to
# > tissues via arterial blood is not depicted explicitly but is considered in the assignment
# > of effective half-times in tissues. The model is intended to yield a conservative average
# > residence time of krypton atoms produced in systemic pools by decay of a parent
# > radionuclide. It is recognised that the residence time of krypton in the body following
# > production in tissues depends on the distribution of the parent radionuclide.

# > (485) ... クリプトンは、Publication 67（ICRP, 1993）で導入されたラドンに関する骨モデルに従うと
# > 仮定されていますが、軟部組織から血液への除去速度については、ラドンに対して仮定された値よりも
# > 高い値が割り当てられています。具体的には、非交換性骨領域、交換性骨領域、または骨表面で
# > 生成されたクリプトンは、それぞれ0.36 d⁻¹、1.5 d⁻¹、100 d⁻¹の速度で血液へ移行します。
# > 軟部組織コンパートメントで生成されたクリプトンは、半減期15分で血液へ移行するのに対し、
# > 軟部組織内での放射性壊変によって生成されるラドンの場合は、半減期30分と仮定されています。
# > 血液に入ったクリプトンは、半減期1分に相当する1000 d⁻¹の速度で体内から除去（呼気により排出）されると
# > 仮定されています。動脈血を介した組織へのクリプトンの再循環は明示的には示されていませんが、
# > 組織における実効半減期を割り当てる際に考慮されています。このモデルは、親放射性核種の壊変により
# > 全身プール内で生成されたクリプトン原子の平均滞留時間について、保守的な値を与えることを
# > 意図しています。組織内での生成後に体内にとどまるクリプトンの滞留時間は、親放射性核種の
# > 分布に依存することが認識されています。

  {Kr}/Blood              Blood                     ---

  {Sr}/Blood              Blood                     ---     # ?
  {Sr}/C-bone-S           Blood                   100
  {Sr}/T-bone-S           Blood                   100
  {Sr}/Exch-C-bone-V      Blood                     1.5
  {Sr}/Exch-T-bone-V      Blood                     1.5
  {Sr}/Noch-C-bone-V      Blood                     0.36
  {Sr}/Noch-T-bone-V      Blood                     0.36

  {Rb}/RBC                Blood                 $(0.693 / (15/60/24))   # 半減期15分
  {Rb}/Muscle             Blood                 $(0.693 / (15/60/24))   # 半減期15分
  {Rb}/OtherTissue        Blood                 $(0.693 / (15/60/24))   # 半減期15分
  {Sr}/ST0                Blood                 $(0.693 / (15/60/24))   # 半減期15分
  {Sr}/ST1                Blood                 $(0.693 / (15/60/24))   # 半減期15分
  {Sr}/ST2                Blood                 $(0.693 / (15/60/24))   # 半減期15分

  {Kr}/Exhaled            Exhaled                   ---
  Blood                   Exhaled                1000   # ≒半減期1分
