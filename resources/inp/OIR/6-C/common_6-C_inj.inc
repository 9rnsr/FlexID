[parameter]
  ${C}/fA    = 0.99 # ≒1   # ICRP Publ.134 p.53 Table 3.3 / Ingested materials, All chemical forms
  ${C}/fA_Re = 0.99 # ≒1   # ICRP Publ.134 p.53 Table 3.3 / footnote††

  ${C}/Blood = 'Blood0'     # Generic model


[include:../share/common_intake_inj.inc]

[include:../share/common_HATM+UB.inc]

[include:element_6-C_Generic.inc]
