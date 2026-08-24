[{Fe}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   OtherPlasma           Blood
  acc   PlasmaTrans           Blood
  acc   RBC                   Blood
  acc   MarrowSynthe          R-marrow
  acc   MarrowTransit         R-marrow
  acc   MarrowStorage         R-marrow
  acc   Liver1(Hepato)        Liver
  acc   Liver2(RE)            Liver
  acc   Spleen                Spleen
  acc   Other1(Trans)         Other
  acc   Other2(Parenc)        Other
  acc   Other3(RE)            Other
  exc   Excreta               ---


[{Fe}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.134 p.143 Table 7.3
  OtherPlasma             PlasmaTrans              70
  OtherPlasma             UB-con                    0.01
  OtherPlasma             RC-con                    0.1
  PlasmaTrans             MarrowSynthe              9.43
  PlasmaTrans             Liver1(Hepato)            0.555
  PlasmaTrans             Other1(Trans)             1.11
  RBC                     OtherPlasma               0.000833
  RBC                     MarrowTransit             0.00729
  RBC                     RC-con                    0.0002
  RBC                     UB-con                    0.000015
  MarrowSynthe            RBC                       0.243
  MarrowSynthe            MarrowTransit             0.104
  MarrowTransit           OtherPlasma               1.39
  MarrowTransit           MarrowStorage             0.0635
  MarrowTransit           Liver2(RE)                0.0106
  MarrowTransit           Spleen                    0.017
  MarrowTransit           Other3(RE)                0.0635
  MarrowStorage           MarrowTransit             0.0038
  Liver2(RE)              MarrowTransit             0.0038
  Spleen                  MarrowTransit             0.0038
  Other3(RE)              MarrowTransit             0.0038
  Liver1(Hepato)          PlasmaTrans               0.00364
  Liver1(Hepato)          SI-conRe                  0.00037
  Other1(Trans)           PlasmaTrans               0.888
  Other1(Trans)           Other2(Parenc)            0.222
  Other2(Parenc)          Other1(Trans)             0.00127
  Other2(Parenc)          Excreta                   0.00057
  Other2(Parenc)          UB-con                    0.00003
