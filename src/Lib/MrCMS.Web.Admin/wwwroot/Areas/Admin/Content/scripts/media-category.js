import {MediaUploader} from "./media-uploader";
import {fileTools} from "./media";

export function setupMediaCategoryPage() {
    $('[data-media-category]').each((index, element) => {
        fileTools().init();
        new MediaUploader($(document), {}).init();
    })
}
