extends Control

@export var audio_stream: AudioStream

signal space_key_pressed

func _input(event: InputEvent) -> void:
	if (event.is_pressed() && event is InputEventKey):
		if (event.keycode == KEY_SPACE):
			space_key_pressed.emit()
		elif (event.keycode == KEY_ENTER):
			GlobalSoundManager.Play(audio_stream)
