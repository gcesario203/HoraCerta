export type SlotHorarioDto = {
  id: string;
  inicio: string;
  fim: string | null;
  status: string;
};

export type CriarSlotRequest = { inicio: string };
