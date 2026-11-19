# ICRP Publ.134 p.223 Para.483
# > (483) The model used in this publication for yttrium as a progeny of strontium is
# > based on the model for yttrium as a parent described elsewhere in this publication,
# > but additional assumptions are made to address structural differences in the strontium
# > and yttrium models. ...

# > (483) 本報告書において、ストロンチウムの娘核種であるイットリウムに対して用いられるモデルは、
# > 同じく本報告書内で記述されている「親核種としてのイットリウム」のモデルに基づいていますが、
# > ストロンチウムのモデルとイットリウムのモデルとの間の構造的な差異に対応するため、追加の
# > 仮定が設けられています。 ...

[{Y}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood1                Blood
  acc   Blood2                Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   C-bone-V              C-bone-V
  acc   T-bone-V              T-bone-V
  acc   Liver0                Liver
  acc   Liver1                Liver
  acc   Kidneys               Kidneys
  acc   ST0                   Other
  acc   ST1                   Other


[{Y}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.134 p.252 Table 11.3
  Blood1                  Blood2                    0.498
  Blood1                  Liver0                    1.66
  Blood1                  Kidneys                   0.166
  Blood1                  ST0                       3.652
  Blood1                  ST1                       1.328
  Blood1                  UB-con                    2.49
  Blood1                  SI-conRe                  0.166
  Blood1                  T-bone-S                  3.32
  Blood1                  C-bone-S                  3.32
  Blood2                  Blood1                    0.462
  Liver0                  SI-conRe                  0.0231
  Liver0                  Blood1                    0.0924
  Liver0                  Liver1                    0.116
  Liver1                  Blood1                    0.0019
  Kidneys                 Blood1                    0.0019
  ST0                     Blood1                    0.231
  ST1                     Blood1                    0.0019
  T-bone-S                Blood1                    0.000493
  T-bone-S                T-bone-V                  0.000247
  T-bone-V                Blood1                    0.000493
  C-bone-S                Blood1                    0.0000821
  C-bone-S                C-bone-V                  0.0000411
  C-bone-V                Blood1                    0.0000821

$if progeny

# ICRP Publ.134 p.223 Para.483
# > (483) ... Yttrium produced in a compartment of bone is assumed to
# > follow the same kinetics as if deposited in the compartment as a parent radionuclide.
# > No distinction is made between the exchangeable and non-exchangeable bone
# > volume compartments of the strontium model when applied to yttrium, i.e. each
# > compartment is treated simply as the bone volume compartment for the corresponding
# > bone type in the yttrium model. Yttrium produced in a soft tissue compartment
# > of the strontium model (ST0, ST1, or ST2) is assumed to transfer to blood with a
# > half-time of 3 d (the shortest removal half-time from compartments of other soft
# > tissue in the model for yttrium as a parent), and then to follow the kinetics of yttrium
# > as a parent radionuclide.
# > (483) ... 骨のコンパートメント内で生成されたイットリウムは、親放射性核種として
# > そのコンパートメントに沈着した場合と同様の動態を示すものと仮定されます。ストロンチウムの
# > モデルにおける「交換可能」および「交換不可能」な骨体積コンパートメントの区別は、
# > イットリウムに適用する際には行われません。すなわち、各コンパートメントは、イットリウムの
# > モデルにおける対応する骨タイプの「骨体積コンパートメント」として単純に扱われます。
# > ストロンチウムのモデルの軟部組織コンパートメント（ST0、ST1、またはST2）内で生成された
# > イットリウムは、3日の半減期（親核種としてのイットリウムのモデルにおける他の軟部組織
# > コンパートメントからの最短の除去半減期）で血液へ移行し、その後、親放射性核種としての
# > イットリウムの動態に従うものと仮定されます。

  {Sr}/Blood              Blood1                    ---
  {Y}/Blood1              Blood1                    ---
  {Y}/Blood2              Blood2                    ---

  {Sr,Y}/C-bone-S         C-bone-S                  ---
  {Sr,Y}/T-bone-S         T-bone-S                  ---
  {Sr}/Exch-C-bone-V      C-bone-V                  ---
  {Sr}/Exch-T-bone-V      T-bone-V                  ---
  {Sr}/Noch-C-bone-V      C-bone-V                  ---
  {Sr}/Noch-T-bone-V      T-bone-V                  ---
  {Y}/C-bone-V            C-bone-V                  ---
  {Y}/T-bone-V            T-bone-V                  ---

  {Y}/Liver0              Liver0                    ---
  {Y}/Liver1              Liver1                    ---
  {Y}/Kidneys             Kidneys                   ---

  {Sr}/ST0                Blood1                    0.23105     # = ln(2)/3
  {Sr}/ST1                Blood1                    0.23105     # = ln(2)/3
  {Sr}/ST2                Blood1                    0.23105     # = ln(2)/3
  {Y}/ST0                 ST0                       ---
  {Y}/ST1                 ST1                       ---

$endif
