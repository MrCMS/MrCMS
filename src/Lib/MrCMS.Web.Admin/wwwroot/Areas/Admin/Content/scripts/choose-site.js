function ensureTargetIsSelf(event) {
    $(event.target).removeAttr('target');
}
export function setupResourceChooseSite(){
    $(document).on('submit', '#resource-choose-site', ensureTargetIsSelf);

}