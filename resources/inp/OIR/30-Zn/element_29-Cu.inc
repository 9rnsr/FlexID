[{Cu}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Plasma                Blood
  acc   RBC                   Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   C-bone-V              C-bone-V
  acc   T-bone-V              T-bone-V
  acc   Liver1                Liver
  acc   Liver2                Liver
  acc   Kidneys               Kidneys
  acc   Pancreas              Pancreas
  acc   Muscle                Muscle
  acc   ST0                   Other
  acc   ST1                   Other
  acc   ST2                   Other
  exc   Excreta               ---


[{Cu}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.134 p.201 Para.436
# > (436) Three isotopes of zinc addressed in this publication have progeny that are
# > considered in the derivation of dose coefficients for the parent radionuclide: 69mZn
# > (t½=13.8 h) decays to 69Zn (56.4 min), 62Zn (9.19 h) decays to 62Cu (9.67 min), and
# > 72Zn (46.5 h) decays to 72Ga (14.1 h). 69Zn presumably behaves the same as the
# > parent radionuclide from the time it is produced in the body. 62Cu produced by
# > decay of 62Zn is assumed to decay at its site of production.
# > (436) この刊行物で取り上げられている3つの亜鉛同位体には、親放射性核種の線量係数の
# > 導出において考慮される子孫核種がある。69mZn（t½=13.8 時間）は 69Zn（56.4 分）に崩壊し、
# > 62Zn（9.19 時間）は 62Cu（9.67 分）に崩壊し、72Zn（46.5 時間）は 72Ga（14.1 時間）に
# > 崩壊する。69Znは体内で生成された時点から親放射性核種と同じように振舞うと考えられる。
# > 62Znの崩壊によって生成された62Cuは、その生成部位で崩壊すると想定される。

  {Zn}/Plasma             Plasma                ---
  {Zn}/RBC                RBC                   ---

  {Zn}/C-bone-S           C-bone-S              ---
  {Zn}/T-bone-S           T-bone-S              ---
  {Zn}/C-bone-V           C-bone-V              ---
  {Zn}/T-bone-V           T-bone-V              ---

  {Zn}/Liver1             Liver1                ---
  {Zn}/Liver2             Liver2                ---
  {Zn}/Kidneys            Kidneys               ---
  {Zn}/Pancreas           Pancreas              ---
  {Zn}/Muscle             Muscle                ---

  {Zn}/ST0                ST0                   ---
  {Zn}/ST1                ST1                   ---
  {Zn}/ST2                ST2                   ---

  {Zn}/Excreta            Excreta               ---
