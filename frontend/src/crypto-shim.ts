export const randomUUID = globalThis.crypto.randomUUID.bind(globalThis.crypto);
export default { randomUUID };
